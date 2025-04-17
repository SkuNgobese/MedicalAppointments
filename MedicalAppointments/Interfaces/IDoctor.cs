using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IDoctor
    {
        ErrorViewModel? Error { get; set; }
        Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();
        Task<ErrorViewModel> EnrollDoctorAsync(Doctor doctor);
        Task<Doctor?> GetDoctorByIdAsync(string id);
        Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital);
        Task<ErrorViewModel> UpdateDoctorAsync(Doctor doctor);
        Task<ErrorViewModel> RemoveDoctorAsync(Doctor doctor);
    }
}