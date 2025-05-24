

using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices
{
    public class ConsultationAppointmetApiService : IConsultationAppointmentService
    {
        mlaraEntities db = new mlaraEntities();
        public readonly IApiResultService _apiResultService = new ResultService();


        /// <inheritdoc/>
        public object GetAvailableAppointment(long consultantId, DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return _apiResultService.Result(false, null, "Date should be greater than or equal to today", null);
            }

            var consultaionAvailablity = db.ConsultationAvailablities.FirstOrDefault();

            if (consultaionAvailablity == null)
            {
                return _apiResultService.Result(false, null, "No Consultation Availablity", null);
            }

            if (!IsConsultantAvailable(
                consultaionAvailablity,
                date,
                consultaionAvailablity.startTime.Add(new TimeSpan(consultaionAvailablity.workingHours, 0, 0)))
              )
            {
                return _apiResultService.Result(false, null, "Consultant is not available on this day", null);
            }


            var appointments = GenerateAppointments(consultaionAvailablity);

            var consultaionAppointmentsToIgnore = GetConsultationAppointmentsToIgnore(consultantId, date);
            var timeNow = DateTime.Now.TimeOfDay;

            if (date.Date == DateTime.Now.Date)
            {
                appointments = appointments
                    .Where(a => a.CompareTo(timeNow) > 0 && IsTimeAvailable(appointments, consultaionAppointmentsToIgnore, a, consultaionAvailablity))
                    .ToList();
            }
            else
            {
                appointments = appointments
                    .Where(a => IsTimeAvailable(appointments, consultaionAppointmentsToIgnore, a, consultaionAvailablity))
                    .ToList();
            }

            if (appointments.Count == 0)
            {
                return _apiResultService.Result(false, null, "No Appointments Available", null);
            }

            return _apiResultService.Result(true, appointments, "Success", new { dateTime = DateTime.Now });
        }

        /// <inheritdoc/>
        public async Task<object> PostBookAppointment(long consultantId, long skillId, string bookingReason, DateTime date, TimeSpan time, long userId)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return _apiResultService.Result(false, null, "Date should be greater than or equal to today", new { dateTime = DateTime.Now });
            }

            var consultaionAvailablity = db.ConsultationAvailablities.FirstOrDefault();

            if (consultaionAvailablity == null)
            {
                return _apiResultService.Result(false, null, "No Consultation Availablity", new { dateTime = DateTime.Now });
            }

            var appointments = GenerateAppointments(consultaionAvailablity);

            if (!IsConsultantAvailable(consultaionAvailablity, date, time))
            {
                return _apiResultService.Result(false, null, "Consultant is not available on this day", new { dateTime = DateTime.Now });
            }

            var userFarmer = db.users.Find(userId);
            var userConsultant = db.users.Find(consultantId);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var consultaionAppointmentsToIgnore = GetConsultationAppointmentsToIgnore(consultantId, date);

                    if (!IsTimeAvailable(appointments, consultaionAppointmentsToIgnore, time, consultaionAvailablity))
                    {
                        return _apiResultService.Result(false, null, "Time is not available", new { dateTime = DateTime.Now });
                    }

                    var appointment = CreateAppointment(consultantId, skillId, date, time, bookingReason, userId);
                                    
                    appointment = db.ConsultationAppointments.Add(appointment);

                    var messageForFarmer = "تم حجز الموعد لطلب استشارة مع المرشد:  " + userConsultant.name + "\n يمكنك مشاهدة مواعديك القادمة في صفحة 'مواعيدي' ";
                    var messageForConsultant = "تم حجز الموعد لطلب استشارة مع المستخدم:  " + userFarmer.name + "\n يمكنك مشاهدة مواعديك القادمة في صفحة 'مواعيدي' ";

                    _ = sendNotification("طلب استشارة", messageForFarmer, new string[] { userFarmer.phone }, userFarmer.id, appointment.Id);
                    _ = sendNotification("طلب استشارة", messageForConsultant, new string[] { userConsultant.phone }, userConsultant.id, appointment.Id);


                    await db.SaveChangesAsync();
                    transaction.Commit();


                    return _apiResultService.Result(true, new { }, "Success", new { dateTime = DateTime.Now });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return _apiResultService.Result(false, null, ex.Message, new { dateTime = DateTime.Now });
                }
            }
        }


        /// <inheritdoc/>
        public object GetMyNextConsultationAppointments(long userId)
        {
            var availablity = db.ConsultationAvailablities.FirstOrDefault();
            var currentDateTime = DateTime.Now;
            var currentTimeMinusDuration = currentDateTime.TimeOfDay.Add(new TimeSpan(0, -availablity.duration, 0));
            var appointments = db.ConsultationAppointments
                .Where(a =>
                    (a.createdBy == userId || a.consultantId == userId) &&
                    (DbFunctions.TruncateTime(a.appointmentDate) > DbFunctions.TruncateTime(currentDateTime) ||
                    (DbFunctions.TruncateTime(a.appointmentDate) == DbFunctions.TruncateTime(currentDateTime) &&
                    TimeSpan.Compare(currentTimeMinusDuration, a.appointmentTime) <= 0)) &&
                     !a.isCanceled
                )
                .OrderBy(a => DbFunctions.TruncateTime(a.appointmentDate))
                .ToList();



            if (appointments.Count == 0)
            {
                return _apiResultService.Result(false, null, "No Appointments Available", null);
            }

            return _apiResultService.Result(
                true,
                appointments.Select(a => generateAppointmentObject(a)).ToList(),
                "Success",
                new { dateTime = currentTimeMinusDuration }
            );
        }

        /// <inheritdoc/>
        public object GetMyHistoryConsultationAppointments(long userId)
        {
            var availablity = db.ConsultationAvailablities.FirstOrDefault();
            var currentDateTime = DateTime.Now;
            var currentTimeWithDuration = currentDateTime.TimeOfDay.Add(new TimeSpan(0, availablity.duration, 0));
            var appointments = db.ConsultationAppointments
                .Where(a =>
                    (a.createdBy == userId || a.consultantId == userId) &&
                    (DbFunctions.TruncateTime(a.appointmentDate) < DbFunctions.TruncateTime(currentDateTime) ||
                    (DbFunctions.TruncateTime(a.appointmentDate) == DbFunctions.TruncateTime(currentDateTime) &&
                    TimeSpan.Compare(currentTimeWithDuration, a.appointmentTime) > 0)) &&
                     !a.isCanceled
                )
                .OrderBy(a => DbFunctions.TruncateTime(a.appointmentDate))
                .ToList();

            if (appointments.Count == 0)
            {
                return _apiResultService.Result(false, null, "No Appointments Available", null);
            }

            return _apiResultService.Result(
                true,
                appointments.Select(a => generateAppointmentObject(a)).ToList(),
                "Success",
                new { dateTime = DateTime.Now }
            );
        }

        /// <inheritdoc/>
        public object PutAppointmentCallDone(long appointmentId, int callDuration)
        {
            var appointment = db.ConsultationAppointments.Find(appointmentId);

            if (appointment == null)
            {
                return _apiResultService.Result(false, null, "Appointment not found", null);
            }

            if (checkAppointmentValidationTime(appointment))
            {
                return _apiResultService.Result(false, null, "Appointment is not started yet", null);
            }

            appointment.callDuration += callDuration;

            db.SaveChanges();

            return _apiResultService.Result(true, null, "Success", null);
        }

        /// <inheritdoc/>
        public object PutAppointmentCallStartBy(long appointmentId, long userId)
        {
            var appointment = db.ConsultationAppointments.Find(appointmentId);

            if (appointment == null)
            {
                return _apiResultService.Result(false, null, "Appointment not found", null);
            }

            if (checkAppointmentValidationTime(appointment))
            {
                return _apiResultService.Result(false, null, "Appointment is not started yet", null);
            }

            appointment.callStartBy = userId;

            db.SaveChanges();

            return _apiResultService.Result(true, null, "Success", null);
        }

        /// <inheritdoc/>
        public object PutCancelAppointmentAsync(long appointmentId, long userId)
        {
            var appointment = db.ConsultationAppointments.Find(appointmentId);

            if (appointment == null)
            {
                return _apiResultService.Result(false, null, "Appointment not found", new { dateTime = DateTime.Now });
            }
            appointment.canceledBy = userId;
            appointment.isCanceled = true;

            db.SaveChanges();

            var messageForFarmer = "تم إلغا حجز الموعد لطلب استشارة مع المرشد:  " + appointment.userConsultant.name + "\n يمكنك مشاهدة مواعديك القادمة في صفحة 'مواعيدي' ";
            var messageForConsultant = "تم إلغا حجز الموعد لطلب استشارة مع المستخدم:  " + appointment.userFarmer.name + "\n يمكنك مشاهدة مواعديك القادمة في صفحة 'مواعيدي' ";

            _ = sendNotification("طلب استشارة", messageForFarmer, new string[] { appointment.userFarmer.phone }, appointment.userFarmer.id, appointmentId);
            _ = sendNotification("طلب استشارة", messageForConsultant, new string[] { appointment.userConsultant.phone }, appointment.userConsultant.id, appointmentId);

            return _apiResultService.Result(true, null, "Success", null);
        }



        private List<TimeSpan> GenerateAppointments(ConsultationAvailablity consultaionAvailablity)
        {
            var appointments = new List<TimeSpan>();
            var appointmentCount = (consultaionAvailablity.workingHours * 60) / consultaionAvailablity.duration;

            for (int i = 0; i < appointmentCount; i++)
            {
                appointments.Add(consultaionAvailablity.startTime.Add(new TimeSpan(0, consultaionAvailablity.duration * i, 0)));
            }

            return appointments;
        }

        private bool IsConsultantAvailable(ConsultationAvailablity consultaionAvailablity, DateTime date, TimeSpan time)
        {
            return consultaionAvailablity.workingDays.Contains(date.DayOfWeek.ToString()) &&
                   !(DateTime.Now.TimeOfDay > time && date.Date == DateTime.Now.Date);
        }

        private List<TimeSpan> GetConsultationAppointmentsToIgnore(long consultantId, DateTime date)
        {
            return db.ConsultationAppointments
                .Where(a => a.consultantId == consultantId && a.appointmentDate == date && !a.isCanceled)
                .Select(a => a.appointmentTime)
                .ToList();
        }

        private bool IsTimeAvailable(List<TimeSpan> appointments, List<TimeSpan> consultaionAppointmentsToIgnore, TimeSpan time, ConsultationAvailablity consultaionAvailablity)
        {
            return appointments
                .Where(a => !consultaionAppointmentsToIgnore.Any(c => c >= a && c < a.Add(new TimeSpan(0, consultaionAvailablity.duration, 0))))
                .Contains(time);
        }

        private ConsultationAppointment CreateAppointment(long consultantId, long skillId, DateTime date, TimeSpan time, string bookingReason, long userId)
        {
            return new ConsultationAppointment
            {
                consultantId = consultantId,
                skillId = skillId,
                appointmentDate = date,
                appointmentTime = time,
                bookingReason = bookingReason,
                createdBy = userId
            };
        }

        private object generateAppointmentObject(ConsultationAppointment appointment)
        {
            return new
            {
                id = appointment.Id,
                consultantName = appointment.userConsultant.name + " " + appointment.userConsultant.last_name,
                skillName = appointment.skill.name,
                appointment.appointmentDate,
                appointment.appointmentTime,
                appointment.bookingReason,
                farmerName = appointment.userFarmer.name + " " + appointment.userFarmer.last_name,
                appointment.createdAt,
                consultantChatId = appointment.userConsultant.chatId,
                farmerChatId = appointment.userFarmer.chatId,
                isTimePassed = appointment.appointmentDate.Date < DateTime.Now.Date || (appointment.appointmentDate.Date == DateTime.Now.Date && appointment.appointmentTime < DateTime.Now.TimeOfDay),
                appointment.isCanceled,
                appointment.callDuration,
                userCallStart = appointment.userCallStart != null ? appointment.userCallStart.name + " " + appointment.userCallStart.last_name : null,
                userCancel = appointment.userCancel != null ? appointment.userCancel.name + " " + appointment.userCancel.last_name : null,
                appointment.cancelReason,
            };
        }

        private bool checkAppointmentValidationTime(ConsultationAppointment appointment)
        {
            var dateTime = DateTime.Now;
            if (appointment.appointmentDate > dateTime.Date ||
                appointment.appointmentDate.Date == dateTime.Date && appointment.appointmentTime > dateTime.TimeOfDay)
            {
                return false;
            }
            return true;
        }

        private async Task<Object> sendNotification(string title, string body, string[] topic, long UserId, long appointmentId)
        {

            Generic g = new Generic();

            await g.sendGeneralNotification(title, body, topic);
            g.InsertNotificationData(UserId, body, (int)AppConstants.Type.Consultation, appointmentId, "");
            return "";

        }
    }
}