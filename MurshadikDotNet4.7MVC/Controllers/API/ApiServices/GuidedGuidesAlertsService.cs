using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices
{
	public class GuidedGuidesAlertsService : IGuidedGuidesAlertsService
	{
		mlaraEntities db = new mlaraEntities();
		public readonly IApiResultService _apiResultService = new ResultService();
		public async Task<object> GetGuidedGuide(long UserId)
		{
			List<GuidedGuideLog> GuidedGuide = await db.GuidedGuideLogs.Where(w => w.consultantId.Equals(UserId)).ToListAsync();
			
				return  GuidedGuide;
			
			
		}
		public async Task<object> PostGuidedGuide(long UserId, string RegonId, string GuideAlert, string[] Cities, string[] Skills)
		{
			try
			{
				string UserLink =db.users.Find(UserId).chatId;
				var Farmer = GetFarmers(RegonId, Cities, Skills);				
				await SendGuidedGuideAsync("تنبية ارشادي", GuideAlert, Farmer.Select(s => s.farmerphone).ToArray());
				var saveNotificationResult = saveNotification(mabGuidAlertToNotification(Farmer.Select(s=>s.farmerId).ToArray(), GuideAlert,UserLink, UserId.ToString()));	
				
				var saveGuidedGuid = saveGuidedGuide(UserId, long.Parse(RegonId),GuideAlert,string.Join(",", Cities) , string.Join(",", Skills),Farmer.Count);

				return _apiResultService.Result(true, "", "Success", false);
			}
			catch (Exception ex)
			{

				return _apiResultService.Result(false, "somthing wrong", "Error", false);
			}
		}
		private List<farmers> GetFarmers(string RegonId, string[]Cities,string[]Skills)
		{
			long regonId = long.Parse(RegonId);
			var farmer = db.users.Where(w => w.region_id == regonId && (w.role_id == 5 || w.role_id == 6));

			if (Cities.Length > 0)
			{
				farmer = farmer.Where(w => Cities.Any(s => w.prefix.Contains(s)));
			}
			if (Skills.Length > 0)
			{
				farmer = farmer.Where(w => Skills.Any(s => w.skills.Contains(s)));

			}
			  string[] farmerphones = farmer.Select(w => w.phone).ToArray();
			List<farmers> farmers= farmer.Select(w => new farmers() {
					farmerId=	w.id,
					farmerphone=w.phone,
					}).ToList();

			return farmers;
		}
		private async Task<Object> SendGuidedGuideAsync(string title,  string GuideAlert, string[] Farmers)
		{

			Generic g = new Generic();
			
			return await g.sendGeneralNotification(title, GuideAlert, Farmers);
		
		}	
		private List<notification> mabGuidAlertToNotification(long[] Farmer,string GuideAlert,string UserLink ,string UserId )
		{			
			return Farmer.Select(id => new notification()
			{
				user_id = id,
				text = GuideAlert,
				type= (int)AppConstants.Type.GuidedGuide,
				type_id = 102,	
				link = UserLink,	
				created_at = DateTime.Now,
			}).ToList();
		}
		private bool saveNotification(List<notification> notifications)
		{
			try
			{
				db.notifications.AddRange(notifications);
				db.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		private bool saveGuidedGuide(long ConsultantId , long RegonId, string GuideAlert, string Cities,string  Skills,int RecipientsCount)
		{
			try
			{

				GuidedGuideLog GuidedGuideLog = new GuidedGuideLog()
				{
					consultantId = ConsultantId,
					guideAlert = GuideAlert,
					regonId = RegonId,
					cities = Cities,
					skills = Skills,
					RecipientsCount = RecipientsCount,
					created_at = DateTime.Now,
				};
				db.GuidedGuideLogs.Add(GuidedGuideLog);
				db.SaveChanges();

				return true;

			}
			catch (Exception)
			{
				return false;
			}
		}	

	}

	public class farmers
	{

		public long farmerId { get; set; }
		public string farmerphone { get; set; }
	}
}