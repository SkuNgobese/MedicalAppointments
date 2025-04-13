using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();

        Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital);

        Task<Doctor> EnrollDoctorAsync(Doctor doctor);

        Task<Doctor?> GetDoctorByIdAsync(string id);

        Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital);

        Task UpdateDoctorAsync(Doctor doctor);

        Task RemoveDoctorAsync(Doctor doctor);

        Task RemoveDoctorsAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);

        Task<Doctor?> GetDoctorAsync(string email);
    }
}