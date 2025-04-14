using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Interfaces
{
    public interface IHospital
    {
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync();

        Task<Hospital> AddHospitalAsync(Hospital hospital);

        Task<Hospital?> GetHospitalByIdAsync(int id);

        Task UpdateHospitalAsync(Hospital hospital);

        Task RemoveHospitalAsync(Hospital hospital);

        Task<Hospital?> GetCurrentUserHospitalAsync();
    }
}