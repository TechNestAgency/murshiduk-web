using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
    /// <summary>
    /// Represents the interface for the Clinic API service.
    /// </summary>
    public interface IClinicApiService
    {
        /// <summary>
        /// Gets all doctors of a specific clinic.
        /// </summary>
        /// <param name="ClinicId">The ID of the clinic.</param>
        /// <returns>An object representing all doctors of the clinic.</returns>
        object GetAllDoctorsOfClinic(int ClinicId);

        /// <summary>
        /// Gets all clinics.
        /// </summary>
        /// <returns>An object representing all clinics.</returns>
        object GetAllClinic();

        /// <summary>
        /// Gets the available appointments for a specific clinic and date.
        /// </summary>
        /// <param name="ClinicId">The ID of the clinic.</param>
        /// <param name="Date">The date for which to get the available appointments.</param>
        /// <returns>An object representing the available appointments.</returns>
        object GetAvailableAppointment(int ClinicId, DateTime Date);

        /// <summary>
        /// Gets the available appointments for a specific doctor of a clinic and date.
        /// </summary>
        /// <param name="ClinicId">The ID of the clinic.</param>
        /// <param name="Date">The date for which to get the available appointments.</param>
        /// <param name="DoctorId">The ID of the doctor.</param>
        /// <returns>An object representing the available appointments.</returns>
        object GetAvailableAppointmentOfDoctor(int ClinicId, DateTime Date, int DoctorId);

        /// <summary>
        /// Books an appointment.
        /// </summary>
        /// <param name="AppointmentId">The ID of the appointment.</param>
        /// <param name="ClinicId">The ID of the clinic.</param>
        /// <param name="BookingReason">The reason for booking the appointment.</param>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>A task representing the booking of the appointment.</returns>
        Task<object> PostBookAppointment(int AppointmentId, int ClinicId, string BookingReason, long UserId);

        /// <summary>
        /// Gets the appointments of a specific user.
        /// </summary>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>An object representing the appointments of the user.</returns>
        object GetMyAppointments(long UserId);

        /// <summary>
        /// Gets the appointment history of a specific user.
        /// </summary>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>An object representing the appointment history of the user.</returns>
        object GetAppoinmetsHistory(long UserId);

        /// <summary>
        /// Marks an appointment as done and updates the call duration.
        /// </summary>
        /// <param name="AppointmentId">The ID of the appointment.</param>
        /// <param name="CallDuration">The duration of the call.</param>
        /// <returns>An object representing the updated appointment.</returns>
        object PutAppointmentCallDone(int AppointmentId, int CallDuration);

        /// <summary>
        /// Cancels a booking for a specific appointment and user.
        /// </summary>
        /// <param name="AppointmentId">The ID of the appointment.</param>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>An object representing the canceled appointment.</returns>
        object PutCancelApointmentBooking(int AppointmentId, long UserId);

        /// <summary>
        /// Posts a rating for a clinic doctor.
        /// </summary>
        /// <param name="ClinicId">The ID of the clinic.</param>
        /// <param name="DonctorId">The ID of the doctor.</param>
        /// <param name="comment">The comment for the rating.</param>
        /// <param name="rating">The rating value.</param>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>An object representing the posted rating.</returns>
        object PostClinicDoctorRating(int ClinicId, long DonctorId, string comment, int rating, long UserId);

        /// <summary>
        /// Checks if a user is a doctor.
        /// </summary>
        /// <param name="UserId">The ID of the user.</param>
        /// <returns>An object representing the result of the check.</returns>
        object IsDoctor(long UserId);
    }
}
