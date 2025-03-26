using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();

        Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital);

        Task<Doctor> EnrollDoctorAsync(Doctor doctor);

        Task<Doctor?> GetDoctorByIdAsync(string id);

        Task UpdateDoctorAsync(Doctor doctor);

        Task RemoveDoctorAsync(Doctor doctor);
    }
}
