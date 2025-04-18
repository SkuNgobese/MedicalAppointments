using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IDoctor
    {
        ErrorViewModel? Error { get; set; }
        Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();
        Task<ErrorViewModel> EnrollDoctorAsync(DoctorViewModel model);
        Task<Doctor?> GetDoctorByIdAsync(string id);
        Task<ErrorViewModel> UpdateDoctorAsync(DoctorViewModel model);
        Task<ErrorViewModel> RemoveDoctorAsync(string id);
    }
}