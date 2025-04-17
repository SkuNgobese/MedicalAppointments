using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();

        Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital);

        Task<Doctor> AddDoctorAsync(Doctor doctor);

        Task<Doctor?> GetDoctorByIdAsync(string id);

        Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital);

        Task UpdateDoctorAsync(Doctor doctor);

        Task DeleteDoctorAsync(Doctor doctor);

        Task DeleteDoctorsAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);

        Task<Doctor?> GetDoctorAsync(string email);

        Task<IEnumerable<Doctor?>> GetCurrentUserHospitalDoctorsAsync();
    }
}