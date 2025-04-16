using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();

        Task<ErrorViewModel> EnrollDoctorAsync(Doctor doctor);

        Task<Doctor?> GetDoctorByIdAsync(string id);

        Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital);

        Task UpdateDoctorAsync(Doctor doctor);

        Task<ErrorViewModel> RemoveDoctorAsync(Doctor doctor);
    }
}