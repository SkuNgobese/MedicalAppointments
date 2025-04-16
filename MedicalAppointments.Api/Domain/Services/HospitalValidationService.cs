using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Services
{
    public class HospitalValidationService : IHospitalValidation
    {
        public ErrorViewModel? CanAddHospital(Hospital hospital, List<Hospital> hospitals)
        {
            if (hospitals.Any(h => h.Name == hospital.Name))
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{hospital.Name} already exists."]
                };
            }

            return null;
        }

        public ErrorViewModel? CanUpdateHospital(Hospital hospital, List<Hospital> hospitals)
        {
            if (hospitals.Any(h => h.Name == hospital.Name))
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation Error",
                    Errors = [$"{hospital.Name} already exists."]
                };
            }

            return null;
        }

        public ErrorViewModel? CanDeleteHospital(Hospital hospital)
        {
            var errors = new List<string>();

            if (hospital.Doctors?.Any(p => p.IsActive) == true)
                errors.Add($"{hospital.Name} has active doctors.");

            if (hospital.Patients?.Any(p => p.IsActive) == true)
                errors.Add($"{hospital.Name} has active patients.");

            if (errors.Count != 0)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Cannot delete hospital",
                    Errors = errors
                };
            }

            return null;
        }
    }
}
