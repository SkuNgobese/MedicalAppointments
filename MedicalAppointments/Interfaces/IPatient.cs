using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IPatient
    {
        Task<IEnumerable<PatientViewModel>> GetAllPatientsAsync();

        Task<Patient> AddPatientAsync(Patient model);

        Task<Patient?> GetPatientByIdNumberOrContactAsync(string term);

        Task<Patient?> GetPatientByIdAsync(string id);

        Task UpdatePatientAsync(Patient patient);

        Task RemovePatientAsync(Patient patient);
    }
}