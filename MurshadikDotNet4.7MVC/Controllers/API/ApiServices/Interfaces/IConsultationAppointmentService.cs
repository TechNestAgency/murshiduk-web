using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
    public interface IConsultationAppointmentService
    {
        object GetAvailableAppointment(long cunsultantId, DateTime date);
        Task<object> PostBookAppointment(long consultantId, long skillId, string bookingReason, DateTime date, TimeSpan time, long userId);
        object GetMyNextConsultationAppointments(long userId);
        object GetMyHistoryConsultationAppointments(long userId);
        object PutAppointmentCallDone(long appointmentId, int callDuration);
        object PutAppointmentCallStartBy(long appointmentId, long userId);
        object PutCancelAppointmentAsync(long appointmentId, long userId);
        
    }
}
