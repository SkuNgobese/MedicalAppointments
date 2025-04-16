using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IHospitalValidation
    {
        ErrorViewModel? CanAddHospital(Hospital hospital, List<Hospital> hospitals);
        ErrorViewModel? CanUpdateHospital(Hospital hospital, List<Hospital> hospitals);
        ErrorViewModel? CanDeleteHospital(Hospital hospital);
    }
}