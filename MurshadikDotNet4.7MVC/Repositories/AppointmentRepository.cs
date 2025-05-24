using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class AppointmentRepository : GenericRepository<appointment>, IAppointmentRepository
    {
        public appointment GetAppointmentByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public object GetAllAppointmentByLabID(long lab_id, string date)
        {
            Array a = new Array[0];

            DateTime dt = Convert.ToDateTime(date);
            DateTime todayDate = DateTime.Now;
            if (dt.Date == todayDate.Date)
            {
                if (DateTime.Now.TimeOfDay.Hours > 20)
                {
                    return a;
                }
                else
                {
                    DateTime time1 = DateTime.Now.AddHours(1);
                    var appointment = _db.appointments.Where(x => x.lab_id == lab_id && x.appointment_date == dt && x.appointment_time > time1.TimeOfDay)
                    .Select(p => new AppointmentDTO
                    {
                        dt = p.appointment_time.Value,
                        is_booked = p.is_booked,
                        id = p.id
                    }).ToList();

                    return appointment;
                }
            }
            else if (dt > todayDate)
            {
                var appointment = _db.appointments.Where(x => x.lab_id == lab_id && x.appointment_date == dt)
                .Select(p => new AppointmentDTO
                {
                    dt = p.appointment_time.Value,
                    is_booked = p.is_booked,
                    id = p.id
                }).ToList();

                return appointment;
            }

            return a;
        }

        public Data AppoinmentBook(long appointment_id, long user_id)
        {
            Data d = new Data();
            appointment appointment = _db.appointments.Where(x => x.id == appointment_id).FirstOrDefault();
            if (appointment != null)
            {
                if (appointment.is_booked == false)
                {
                    appointment.user_id = user_id;
                    appointment.is_booked = true;
                    appointment.created_at = DateTime.Now;
                    Update(appointment);
                    Save();

                    d.status = true;
                    d.message = "Successfully Created Appointment!";
                    d.data = "Your Appointment ID - : " + appointment_id.ToString();
                    return d;
                }
                else
                {
                    d.status = false;
                    d.message = "appointment is already exists!";
                    d.data = "Error";
                    return d;
                }
            }
            else
            {
                d.status = false;
                d.message = "appointment is not exists!";
                d.data = "Error";
                return d;
            }
        }

        public Data CancelAppointment(long appointment_id, long userid)
        {
            Data d = new Data();
            appointment appointment = GetAll().Where(x => x.id == appointment_id && x.user_id == userid).FirstOrDefault();
            if (appointment != null)
            {
                if (appointment.is_completed == false)
                {
                    if (appointment.no_samples == 0)
                    {
                        appointment.user_id = null;
                        appointment.is_booked = false;
                        appointment.updated_at = DateTime.Now;
                        Update(appointment);
                        Save();

                        d.message = "تم إلغاء موعدك";
                        d.status = true;
                        d.data = null;


                        user u = _db.users.Where(x => x.id == userid).FirstOrDefault();

                        Generic g = new Generic();
                        g.sendNotificationOnChats(u.name + " " + u.last_name, "تم إلغاء موعدك", u.phone, "lab_appointment_cancelled", appointment.lab_id.ToString());

                        return d;
                    }
                    else
                    {
                        d.message = "لايمكن إلغاء الموعد يتم فحص العينات المستلمة";
                        d.status = true;
                        d.data = null;
                        return d;
                    }
                }
                else
                {
                    d.message = "لايمكن إلغاء الموعد تم الإنتهاء من تحليل العينة";
                    d.status = true;
                    d.data = null;
                    return d;
                }
            }
            else
            {
                d.message = "لم يتم العثور على موعد مقابل معرّف الموعد هذا!";
                d.status = true;
                d.data = null;
                return d;
            }
        }
    }
}