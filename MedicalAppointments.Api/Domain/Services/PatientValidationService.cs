using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Services
{
    public class PatientValidationService : IPatientValidation
    {
        public ErrorViewModel? CanAddPatient(Patient patient, List<Patient> patients)
        {
            if (patients.Any(p => p.FullName == patient.FullName && p.IDNumber == patient.IDNumber))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{patient.FullName} already exists as patient"]
                };

            return null;
        }

        public ErrorViewModel? CanRemovePatient(Patient patient)
        {
            if (patient.Appointments != null && patient.Appointments.Any(a => a.Date > DateTime.Now && a.Status != AppointmentStatus.Cancelled))
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{patient.FullName} has pending appointments"]
                };

            return null;
        }

        public ErrorViewModel? CanUpdatePatient(Patient patient)
        {
            return null;
        }
    }
}
