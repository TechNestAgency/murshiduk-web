using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using NPOI.HSSF.Record;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace MurshadikCP.Controllers.API.ApiServices
{
	public class ClinicApiService : IClinicApiService
	{
		mlaraEntities db = new mlaraEntities();
		public readonly IApiResultService _apiResultService = new ResultService();
		string hostURL = WebConfigurationManager.AppSettings["publicHostUrl"];

        public object GetAllDoctorsOfClinic(int clinicId)
        {
            var doctors = db.Doctors
				.Where(w => w.ClinicDoctors.Any(a => a.ClinicId == clinicId))
				.Select(s => new
            {
				id = s.Id,
				name = s.user.name + " " + s.user.last_name,
				avatar = s.user.avatar,
				rating = s.DoctorRatings.Count > 0 ? s.DoctorRatings.Average(a => a.Rating) : 0,
            }).ToList();
			return _apiResultService.Result(true, doctors, "Success", new { dateTime = DateTime.Now });
        }

        public object GetAllClinic()
		{
			var Clinics = db.Clinics.Select(s => new
			{
				id = s.Id,
				name = s.Name,
				description = s.Description,
				icon = s.Img,
			}).ToList();
			return _apiResultService.Result(true, Clinics, "Success", new { dateTime = DateTime.Now });

			
		}
		public object GetAvailableAppointment(int ClinicId, DateTime Date)
		{
			var Appointmet = (from clinicAppointment in db.ClinicAppointments
							  join doctClinic in db.ClinicDoctors on ClinicId equals doctClinic.ClinicId
							  where clinicAppointment.DoctorId == doctClinic.DoctorId && clinicAppointment.IsBooked == false && EntityFunctions.TruncateTime(clinicAppointment.AppointmentDate) == Date.Date
							  select new
							  {
								  id = clinicAppointment.Id,
								  AppointmentTime = clinicAppointment.AppointmentTime,

							  }).ToList();
			return _apiResultService.Result(true, Appointmet, "Success", new { dateTime = DateTime.Now });

		}

        public object GetAvailableAppointmentOfDoctor(int ClinicId, DateTime Date, int DoctorId)
        {
            var Appointmet = (from clinicAppointment in db.ClinicAppointments
                              where clinicAppointment.DoctorId == DoctorId && clinicAppointment.IsBooked == false && EntityFunctions.TruncateTime(clinicAppointment.AppointmentDate) == Date.Date
                              select new
                              {
                                  id = clinicAppointment.Id,
                                  AppointmentTime = clinicAppointment.AppointmentTime,

                              }).ToList();
            return _apiResultService.Result(true, Appointmet, "Success", new { dateTime = DateTime.Now });

        }
        public async Task<object> PostBookAppointment(int AppointmentId, int ClinicId, string BookingReason, long UserId)
		{
			var Appointment = db.ClinicAppointments.FirstOrDefault(a => a.Id == AppointmentId && a.IsBooked == false);
			if (Appointment != null)
			{
				Appointment.ClinicId = ClinicId;
				Appointment.BookingResone = BookingReason;
				Appointment.CustmerId = UserId;
				Appointment.IsBooked = true;
				Appointment.CallDuration = 0;
				db.Entry(Appointment).State = EntityState.Modified;
				db.SaveChanges();

				var DoctorData = db.users.Find(Appointment.Doctor.UserId);
				var CustomerData = db.users.Find(UserId);

				var CustomerNotification =await sendNotification("العيادات الإفتراضية","تم حجز موعدك لعيادة :  " + Appointment.Clinic.Name, new string[] { CustomerData.phone }, CustomerData.id);	
				var DoctNotification =await sendNotification("العيادات الإفتراضية",  "لديك موعد جديد في عيادتك  الطبية:  "+Appointment.Clinic.Name, new string[] { DoctorData.phone }, DoctorData.id);


				return _apiResultService.Result(true, new {
					doctor_name = DoctorData.name +" "+ DoctorData.last_name,
					doctor_id = DoctorData.id,
					doctor_chatId= DoctorData.chatId
				}, "Success", false);
				
			}
			return _apiResultService.Result(false, new { }, "Error", false);
		}

		public object GetMyAppointments(long UserId)
		{
			object appointments = new object();
			var date = DateTime.Now;
			var Doctor = db.Doctors.Where(w => w.UserId == UserId).FirstOrDefault();
			if (Doctor == null)
			{
				appointments = (from appointment in db.ClinicAppointments
								join UserAsDoctor in db.users on appointment.Doctor.UserId equals UserAsDoctor.id
								where appointment.CustmerId == UserId && appointment.IsBooked == true && appointment.AppointmentDate >= date.Date 
								select new
								{
									id = appointment.Id,
									name = UserAsDoctor.name + " " + UserAsDoctor.last_name,
									avatar = UserAsDoctor.avatar,
									clinic_name = appointment.Clinic.Name,
									clinic_icon = appointment.Clinic.Img,
									date = appointment.AppointmentDate,
									time = appointment.AppointmentTime,
									chatId = UserAsDoctor.chatId,
									resone = appointment.BookingResone,
									call_duration = appointment.CallDuration,
								}).ToList();
				if (appointments == null)
					appointments = "".ToList();


				return _apiResultService.Result(true, appointments, "Success",new { dateTime = DateTime.Now});
			}
			appointments = (from appointment in db.ClinicAppointments
							join UserAsDoctor in db.users on appointment.Doctor.UserId equals UserAsDoctor.id
							join UserAsCustmor in db.users on appointment.CustmerId equals UserAsCustmor.id

							where appointment.DoctorId == Doctor.Id && appointment.IsBooked == true && appointment.AppointmentDate >= date.Date 
							select new
							{
								id = appointment.Id,
								name = UserAsCustmor.name + " " + UserAsCustmor.last_name,
								avatar = UserAsCustmor.avatar,
								clinic_name = appointment.Clinic.Name,
								clinic_icon = appointment.Clinic.Img,
								date = appointment.AppointmentDate,
								time = appointment.AppointmentTime,
								chatId = UserAsCustmor.chatId,
								resone = appointment.BookingResone,
								call_duration = appointment.CallDuration,


							}).ToList();
			if (appointments == null)
				appointments = "".ToList();

			return _apiResultService.Result(true, appointments, "Success", new { dateTime = DateTime.Now });

		}
		public object GetAppoinmetsHistory(long UserId)
		{
			object appointments = new object();
			var date = DateTime.Now;

			var Doctor = db.Doctors.Where(w => w.UserId == UserId).FirstOrDefault();

			if (Doctor == null)
			{
				appointments = (from appointment in db.ClinicAppointments
								join UserAsDoctor in db.users on appointment.Doctor.UserId equals UserAsDoctor.id
								where appointment.CustmerId == UserId && appointment.IsBooked == true && EntityFunctions.TruncateTime(appointment.AppointmentDate) <= date.Date 
								select new
								{
									id = appointment.Id,
									name = UserAsDoctor.name + " " + UserAsDoctor.last_name,
									avatar = UserAsDoctor.avatar,
									clinic_name = appointment.Clinic.Name,
									clinic_icon = appointment.Clinic.Img,
									date = appointment.AppointmentDate,
									time = appointment.AppointmentTime,
									chatId = UserAsDoctor.chatId,
									resone = appointment.BookingResone,

									call_duration = appointment.CallDuration,
								}).ToList();
				if (appointments == null)
					appointments = "".ToList();


				return _apiResultService.Result(true, appointments, "Success", new { dateTime = DateTime.Now });
			}
			appointments = (from appointment in db.ClinicAppointments
							join UserAsDoctor in db.users on appointment.Doctor.UserId equals UserAsDoctor.id
							join UserAsCustmor in db.users on appointment.CustmerId equals UserAsCustmor.id
							where appointment.DoctorId == Doctor.Id && appointment.IsBooked == true && EntityFunctions.TruncateTime(appointment.AppointmentDate) <= date.Date 
							select new
							{
								id = appointment.Id,
								name = UserAsCustmor.name + " " + UserAsCustmor.last_name,
								avatar = UserAsCustmor.avatar,
								clinic_name = appointment.Clinic.Name,
								clinic_icon = appointment.Clinic.Img,
								date = appointment.AppointmentDate,
								time = appointment.AppointmentTime,
								chatId = UserAsCustmor.chatId,
								resone = appointment.BookingResone,

								call_duration = appointment.CallDuration,

							}).ToList();
			if (appointments == null)
				appointments = "".ToList();


			return _apiResultService.Result(true, appointments, "Success", new { dateTime = DateTime.Now });

		}
		public object PutAppointmentCallDone(int AppointmentId, int CallDuration)
		{
			var Appointment = db.ClinicAppointments.FirstOrDefault(a => a.Id == AppointmentId && a.IsBooked == true);
			if (Appointment != null)
			{
				Appointment.CallDuration += CallDuration;
				db.Entry(Appointment).State = EntityState.Modified;
				db.SaveChanges();
				return _apiResultService.Result(true, new { }, "Success", false);
			}
			return _apiResultService.Result(false, new { }, "Error", false);
		}
		public object PostClinicDoctorRating(int ClinicId, long DoctorId, string comment, int rating, long UserId)
		{
			db.DoctorRatings.Add(new DoctorRating
			{
				Comment = comment,
				Rating = rating,
				CreateBy = UserId,
				DoctorId = DoctorId,
				CreatedAt = DateTime.Now,
			});
			db.SaveChanges();
			return _apiResultService.Result(true, new { }, "Success", false);

		}

		public object PutCancelApointmentBooking(int AppointmentId, long UserId)
		{
			var Appointment = db.ClinicAppointments.FirstOrDefault(a => a.Id == AppointmentId && a.IsBooked == true);
			if (Appointment != null)
			{

				Appointment.IsCanceled = true;
				Appointment.CustmerId = null;
				Appointment.BookingResone = null;
				Appointment.CallDuration = 0;

				db.Entry(Appointment).State = EntityState.Modified;
				db.SaveChanges();
				return _apiResultService.Result(true, new { }, "Success", false);

			}
			return _apiResultService.Result(false, new { }, "Error", false);
		}

		public object IsDoctor(long UserId)
		{
			var isDoctor = db.Doctors.Any(a => a.UserId == UserId);
			if (isDoctor)
			{
				return _apiResultService.Result(true, new { }, "Success", false);
			}
			return _apiResultService.Result(false, new { }, "Error", false);
		}


		private async Task<Object> sendNotification(string title, string body, string[] topic,long UserId)
		{

			Generic g = new Generic();

			await g.sendGeneralNotification(title, body, topic);
			g.InsertNotificationData(UserId,body, (int)AppConstants.Type.Clinic,100,"");
			return "";



		}

       
    }
}
