using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface IPatient
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital);

        Task AddPatientAsync(Patient patient);

        Task<Patient?> GetPatientByIdAsync(string id);

        Task UpdatePatientAsync(Patient patient);

        Task RemovePatientAsync(Patient patient);
        Task RemovePatientsAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);
        Task<Patient?> GetPatientAsync(string email);
    }
}