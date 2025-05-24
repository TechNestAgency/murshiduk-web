using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiServices;
using System.Web.Http;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

namespace MurshadikCP.Controllers.API
{
    [RoutePrefix("Api/Murshadik/ConsultAppointment")]
    public class ConsultationAppointmentController : ApiController
    {
        public readonly IApiResultService _apiResultService = new ResultService();
        public readonly IConsultationAppointmentService _consultationAppointmentService = new ConsultationAppointmetApiService();
        [Authorize]
        [HttpGet]
        [Route("GetAvailableTimes")]
        public object GetAvailableTimes(long consultatntId, string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return _apiResultService.Result(false, null, "Date is required", new { dateTime = DateTime.Now });
            }
            DateTime dateTime = DateTime.Parse(date);
            return _consultationAppointmentService.GetAvailableAppointment(consultatntId, dateTime);
        }

        [Authorize]
        [HttpPost]
        [Route("postBookAppointment")]
        public async Task<object> PostBookAppointmentAsync(long consultantId, long skillId, string bookingReason, string date, string time)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
            {
                return _apiResultService.Result(false, null, "Date and Time are required", new { dateTime = DateTime.Now });
            }
            DateTime dateTime = DateTime.Parse(date);
            TimeSpan timeSpan = TimeSpan.Parse(time);

            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);


            return await _consultationAppointmentService.PostBookAppointment(consultantId, skillId, bookingReason, dateTime, timeSpan, UserId);
        }

        [Authorize]
        [HttpGet]
        [Route("GetMyNextConsultationAppointments")]
        public object GetMyNextConsultationAppointments()
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);

            return _consultationAppointmentService.GetMyNextConsultationAppointments(UserId);
        }

        [Authorize]
        [HttpGet]
        [Route("GetMyHistoryConsultationAppointments")]
        public object GetMyHistoryConsultationAppointments()
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);

            return _consultationAppointmentService.GetMyHistoryConsultationAppointments(UserId);
        }

        [Authorize]
        [HttpPut]
        [Route("PutAppointmentCallDone")]
        public object PutAppointmentCallDone(long appointmentId, int callDuration)
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);

            return _consultationAppointmentService.PutAppointmentCallDone(appointmentId, callDuration);
        }

        [Authorize]
        [HttpPut]
        [Route("PutAppointmentCallStartBy")]
        public object PutAppointmentCallStartBy(long appointmentId)
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);

            return _consultationAppointmentService.PutAppointmentCallStartBy(appointmentId, UserId);
        }

        [Authorize]
        [HttpPut]
        [Route("PutCancelAppointment")]
        public object PutCancelAppointment(long appointmentId)
        {
            var identity = User.Identity as ClaimsIdentity;
            string user_id = identity.Claims.FirstOrDefault(x => x.Type.Equals("userid")).Value;
            var UserId = long.Parse(user_id);

            return _consultationAppointmentService.PutCancelAppointmentAsync(appointmentId, UserId);
        }

        
    }
}
