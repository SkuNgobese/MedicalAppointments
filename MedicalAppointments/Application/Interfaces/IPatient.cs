using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IPatient
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        Task AddPatientAsync(Patient patient);

        Task<Patient?> GetPatientByIdAsync(string id);

        Task UpdatePatientAsync(Patient patient);

        Task RemovePatientAsync(string id);
    }
}
