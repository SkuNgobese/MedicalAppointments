using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Services
{
    public class AppointmentValidationService : IAppointmentValidation
    {
        public ErrorViewModel? CanSchedule(DateTime date, Doctor? doctor, Patient? patient)
        {
            if (date < DateTime.Now)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"Appointment date cannot be in the past."]
                };

            if (doctor == null)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"Doctor is required to schedule an appointment."]
                };
            
            if (patient == null)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"Patient is required to schedule an appointment."]
                };

            if (!doctor.IsActive)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{doctor.FullName} is not available for appointments."]
                };

            if (doctor.Appointments != null &&
                doctor.Appointments.Count(a => a.Date.Date == date) >= 10)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{doctor.FullName} is fully booked on {date:dd/MMM/yyyy}."]
                };

            if (doctor.Appointments != null && doctor.Appointments.Any(a => a.Date == date && 
                a.Status != AppointmentStatus.Cancelled))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{doctor.FullName} is already booked for {date:dd/MMM/yyyy HH:mm}."]
                };

            patient.Appointments ??= [];

            if (patient.Appointments.Any(a => a.Date == date && a.Doctor == doctor))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{patient.FullName} already has an appointment booked for " +
                              $"{date:dd/MMM/yyyy HH:mm} with {doctor.FullName}."]
                };

            return null;
        }

        public ErrorViewModel? CanReschedule(DateTime newDate, Appointment appointment) => 
            CanSchedule(newDate, appointment.Doctor, appointment.Patient);

        public ErrorViewModel? CanCancel(Appointment appointment)
        {
            if (appointment.Date < DateTime.Now)
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"No need to cancel past appointment.."]
                };

            return null;
        }

        public ErrorViewModel? CanReassign(Doctor newDoctor, Appointment appointment) => 
            CanSchedule(appointment.Date, newDoctor, appointment.Patient);
    }
}
