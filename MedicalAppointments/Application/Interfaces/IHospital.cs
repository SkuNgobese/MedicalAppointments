using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IHospital
    {
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync();

        Task AddHospitalAsync(Hospital hospital);

        Task<Hospital?> GetHospitalByIdAsync(int id);

        Task UpdateHospitalAsync(Hospital hospital);

        Task RemoveHospitalAsync(int id);
    }
}
