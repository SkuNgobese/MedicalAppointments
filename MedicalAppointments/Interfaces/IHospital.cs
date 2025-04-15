using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IHospital
    {
        Task<IEnumerable<HospitalViewModel>> GetAllHospitalsAsync();

        Task<Hospital> AddHospitalAsync(Hospital hospital);

        Task<Hospital?> GetHospitalByIdAsync(int id);

        Task UpdateHospitalAsync(Hospital hospital);

        Task RemoveHospitalAsync(Hospital hospital);
    }
}