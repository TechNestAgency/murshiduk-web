using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class LabRepository : GenericRepository<lab>, ILabRepository
    {
        public lab GetLabByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public object GetLab()
        {
            var labs = _db.labs.Include("city").Where(x => x.active == true)
                .Select(p => new
                {
                    p.id,
                    p.Name,
                    p.city
                })
                .ToList();

            return labs;
        }

        public object GetLabData()
        {
            var regions = _db.regions.Where(x => x.active == true).Select(p => new { p.id, p.name_ar, p.name }).ToList();
            var labs = _db.labs.Include("city").Where(x => x.active == true)
                .Select(p => new
                {
                    p.id,
                    p.Name,
                    p.city
                })
                .ToList();

            return new { regions, labs };
        }

        public List<lab_reports> GetLabReportByID(long appointment_id)
        {
            return _db.lab_reports.Where(x => x.appointment_id == appointment_id).ToList();
        }

        public Data GetLabReport()
        {
            Data d = new Data();
            var query = from lr in _db.lab_reports
                        join l in _db.labs on lr.lab_id equals l.id
                        select new
                        {
                            lr,
                            l
                        };

            if (query != null)
            {
                d.message = "successfully fetch the records";
            }
            else
            {
                d.message = "no data found against this appointment id";
            }
            d.data = query.FirstOrDefault();
            d.status = true;
            return d;
        }

        public Object GetMyLabAppointment(long userid)
        {
            return _db.fun_GetAllMyLabAppointment(userid);
        }

        public Data GetAllAppointmentByUser(long userid)
        {
            Data d = new Data();
            List<appointment> app = new List<appointment>();
            
            if (_db.appointments.Where(x => x.user_id == userid).Count() > 0)
            {
                app = _db.appointments.Where(x => x.user_id == userid).ToList();
                d.message = "successfully fetch the appointment data";
            }
            else
            {
                d.message = "no appointment data against this user";
            }

            d.status = true;
            d.data = app;
            return d;
        }
    }
}