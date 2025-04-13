using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IHospitalValidation
    {
        bool CanAddHospital(Hospital hospital, List<Hospital> hospitals);
        bool CanUpdateHospital(Hospital hospital, List<Hospital> hospitals);
        bool CanDeleteHospital(Hospital hospital);
    }
}