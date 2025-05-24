using MurshadikCP.Controllers.Dashboard.DashboardInterface;
using MurshadikCP.Controllers.Dashboard.DashboardViewModel;
using MurshadikCP.Models.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MurshadikCP.Controllers.Dashboard.DashboardServices
{
	public class GuideAlertService : IGuideAlert
	{
		private mlaraEntities db = new mlaraEntities();
		public   IPagedList<GuideAlertViewModel> GetGuideAlert(int pagesize ,int No_Of_Page)
		{

			

			var guideAlerts = (from guideAlert in db.GuidedGuideLogs
					   join user in db.users on guideAlert.consultantId equals user.id
					   join region in db.regions on guideAlert.regonId equals region.id
					   select new GuideAlertViewModel()
					   {
						   GuideName = user.name+" "+user.last_name,	
						   GuidePhonNumber = user.phone,
						   Alert=guideAlert.guideAlert,
						   Regon =region.name_ar,
						   Skills=guideAlert.skills,
						   Cities=guideAlert.cities,
						   RecipientsCount = guideAlert.RecipientsCount,
						   Created_At = guideAlert.created_at,
					   }).OrderByDescending(x=>x.Created_At).ToPagedList(No_Of_Page, pagesize);

			return guideAlerts;
		}
	}
}