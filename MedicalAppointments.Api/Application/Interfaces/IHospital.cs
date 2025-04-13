using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Interfaces
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