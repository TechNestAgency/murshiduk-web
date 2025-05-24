using MurshadikCP.Controllers.API.ApiServices;
using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Models.DB;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
namespace MurshadikCP.Controllers.API
{
	[RoutePrefix("Api/Murshadik/Clinic")]
	public class ClinicController : ApiController
	{
		public readonly IApiResultService _apiResultService = new ResultService();
		public readonly IClinicApiService _clinicApiService = new ClinicApiService();
		[Authorize]
		[HttpGet]
		[Route("getClinics")]
		public  object GetAll()
		{	
			return _clinicApiService.GetAllClinic();	
		}

        [Authorize]
        [HttpGet]
        [Route("getAllDoctorsOfClinic")]
        public object getAllDoctorsOfClinic(int clinic_id)
        {
            return _clinicApiService.GetAllDoctorsOfClinic(clinic_id);
        }

        [Authorize]
		[HttpGet]
		[Route("getAvailableAppointment")]
		public object getAvailableAppointment(int clinic_id,string date)
		{
			DateTime dateTime = DateTime.Parse(date);
			return  _clinicApiService.GetAvailableAppointment(clinic_id,dateTime);	
		}

        [Authorize]
        [HttpGet]
        [Route("getAvailableAppointmentOfDoctor")]
        public object getAvailableAppointmentOfDoctor(int clinic_id, string date, int doctor_id)
        {
            DateTime dateTime = DateTime.Parse(date);
            return _clinicApiService.GetAvailableAppointmentOfDoctor(clinic_id, dateTime, doctor_id);
        }

        [Authorize]
		[HttpGet]
		[Route("getMyAppointments")]
		public object getMyAppointments(bool? is_doctor)
		{
			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return  _clinicApiService.GetMyAppointments(UserId);	
		}
		[Authorize]
		[HttpGet]
		[Route("getAppoinmetsHistory")]
		public object getAppoinmetsHistory(bool? is_doctor)
		{
			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return _clinicApiService.GetAppoinmetsHistory(UserId);	
		}
		[Authorize]
		[HttpPost]
		[Route("postBookAppointment")]
		public async Task<object> postBookAppointmentAsync(int clinic_id,string booking_reason,int appointment_id)
		{
			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return await _clinicApiService.PostBookAppointment(appointment_id,clinic_id, booking_reason,UserId);	
		}
		[Authorize]
		[HttpPut]
		[Route("putAppointmentCallDone")]
		public object putAppointmentCallDone(int call_duration,int appointment_id)

		{
			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return _clinicApiService.PutAppointmentCallDone(appointment_id, call_duration);	
		}
		[Authorize]
		[HttpPost]
		[Route("postClinicDoctorRating")]
		public object postClinicDoctorRating(int clinic_id, long doctor_id ,string comment, int rating)
		{

			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return _clinicApiService.PostClinicDoctorRating(clinic_id, doctor_id, comment ,rating,UserId);	
		}
		[Authorize]
		[HttpPut]
		[Route("putCancelApointmentBooking")]
		public object putCancelApointmentBooking(int AppointmentId )
		{

			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return _clinicApiService.PutCancelApointmentBooking(AppointmentId, UserId);	
		}
		[Authorize]
		[HttpGet]
		[Route("isDoctor")]
		public object isDoctor()
		{

			var identity = User.Identity as ClaimsIdentity;
			string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
			var UserId = long.Parse(user_id);
			return _clinicApiService.IsDoctor(UserId);	
		}
		[Authorize]
		[HttpGet]
		[Route("getIsAppointmentTimeUp")]
		public object getIsAppointmentTimeUp()

		{


			return _apiResultService.Result(true,DateTime.Now, "success", false);
		}

	}
}