using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IHospitalValidation
    {
        bool CanAddHospital(Hospital hospital, List<Hospital> hospitals);
        bool CanUpdateHospital(Hospital hospital, List<Hospital> hospitals);
        bool CanDeleteHospital(Hospital hospital);
    }
}
