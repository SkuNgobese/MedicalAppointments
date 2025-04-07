using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Interfaces
{
    public interface IPatient
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital);

        Task AddPatientAsync(Patient patient);

        Task<Patient?> GetPatientByIdAsync(string id);

        Task UpdatePatientAsync(Patient patient);

        Task RemovePatientAsync(Patient patient);
    }
}