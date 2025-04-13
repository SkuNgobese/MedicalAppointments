using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Models;
using MedicalAppointments.Api.Enums;

namespace MedicalAppointments.Api.Domain.Services
{
    public class AppointmentValidationService : IAppointmentValidation
    {
        public bool CanSchedule(DateTime date, Doctor? doctor, Patient? patient)
        {
            if (date < DateTime.Now)
                throw new ArgumentException("Appointment date cannot be in the past.");

            if (doctor == null)
                throw new ArgumentException("Doctor is required to schedule an appointment.");
            
            if (patient == null)
                throw new ArgumentException("Patient is required to schedule an appointment.");

            if (!doctor.IsActive)
                throw new ArgumentException($"{doctor.FullName} is currently unavailable.");

            if (doctor.Appointments != null && doctor.Appointments.Count == 10)
                throw new ArgumentException($"{doctor.FullName} is fully booked.");

            if (doctor.Appointments != null && doctor.Appointments.Any(a => a.Date == date && a.Status != AppointmentStatus.Cancelled))
                throw new ArgumentException($"{doctor.FullName} is already booked for {date:dd/MMM/yyyy HH:mm}.");

            patient.Appointments ??= [];

            if (patient.Appointments.Any(a => a.Date == date && a.Doctor == doctor))
                throw new ArgumentException($"{patient.FullName} already has an appointment booked for " +
                                            $"{date:dd/MMM/yyyy HH:mm} with {doctor.FullName}.");

            return true;
        }

        public bool CanReschedule(DateTime newDate, Appointment appointment)
        {
            return CanSchedule(newDate, appointment.Doctor, appointment.Patient);
        }

        public bool CanCancel(Appointment appointment)
        {
            if (appointment.Date < DateTime.Now)
                throw new ArgumentException("Appointment cannot be cancelled because it is in the past.");

            return true;
        }

        public bool CanReassign(Doctor newDoctor, Appointment appointment)
        {
            return CanSchedule(appointment.Date, newDoctor, appointment.Patient);
        }
    }
}
