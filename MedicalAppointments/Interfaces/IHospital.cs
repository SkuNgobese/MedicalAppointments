using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IHospital
    {
        Task<IEnumerable<HospitalViewModel>> GetAllHospitalsAsync();

        Task<ErrorViewModel> AddHospitalAsync(Hospital hospital);

        Task<Hospital?> GetHospitalByIdAsync(int id);

        Task<ErrorViewModel> UpdateHospitalAsync(Hospital hospital);

        Task<ErrorViewModel> RemoveHospitalAsync(Hospital hospital);
    }
}