using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IHospital
    {
        ErrorViewModel? Error { get; set; }
        Task<IEnumerable<HospitalViewModel>> GetAllHospitalsAsync();
        Task<ErrorViewModel> AddHospitalAsync(Hospital hospital);
        Task<HospitalViewModel?> GetHospitalByIdAsync(int id);
        Task<ErrorViewModel> UpdateHospitalAsync(HospitalViewModel model);
        Task<ErrorViewModel> RemoveHospitalAsync(int hospitalId);
    }
}