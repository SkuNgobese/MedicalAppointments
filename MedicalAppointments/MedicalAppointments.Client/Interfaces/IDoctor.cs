using MedicalAppointments.Client.Models;

namespace MedicalAppointments.Client.Interfaces
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
    }
}
