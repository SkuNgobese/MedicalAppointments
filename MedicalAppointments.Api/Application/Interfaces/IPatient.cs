using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IPatient
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital);

        Task<IEnumerable<Patient>> GetAllPatientsAsync(Doctor doctor);

        Task<Patient> AddPatientAsync(Patient patient);

        Task<Patient?> GetByIdNumberOrContactAsync(Hospital hospital, string search);

        Task<Patient?> GetPatientByIdAsync(string id);

        Task UpdatePatientAsync(Patient patient);

        Task DeletePatientAsync(Patient patient);

        Task RemovePatientsAsync(Doctor doctor);

        Task DeletePatientsAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);
        Task<Patient?> GetPatientAsync(string email);

        Task<IEnumerable<Patient?>> GetCurrentUserHospitalPatientsAsync();
    }
}