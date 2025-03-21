using MedicalAppointments.Application.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IDoctor
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();

        Task EnrollDoctorAsync(Doctor doctor);

        Task<Doctor?> GetDoctorByIdAsync(string id);

        Task UpdateDoctorAsync(Doctor doctor);

        Task RemoveDoctorAsync(string id);
    }
}
