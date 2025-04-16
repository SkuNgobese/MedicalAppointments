using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Services
{
    public class DoctorValidationService : IDoctorValidation
    {
        public ErrorViewModel? CanAdd(Doctor doctor, List<Doctor> doctors)
        {
            if (doctors.Any(d => d.FullName == doctor.FullName && d.IDNumber == doctor.IDNumber))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{doctor.FullName} already exists."]
                };

            return null;
        }

        public ErrorViewModel? CanRemove(Doctor doctor, DateTime removeDate = default)
        {
            removeDate = removeDate == default ? DateTime.Now : removeDate;

            if (doctor.Appointments != null && doctor.Appointments.Any(a => a.Date > removeDate && a.Status != AppointmentStatus.Cancelled))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{doctor.FullName} has coming appointments."]
                };

            return null;
        }

        public bool CanRetire(Doctor doctor, DateTime retireDate) =>
            CanRemove(doctor, retireDate) == null;

        public ErrorViewModel? CanUpdate(Doctor doctor, List<Doctor> doctors) =>
            null;
    }
}
