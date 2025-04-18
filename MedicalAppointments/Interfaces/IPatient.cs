using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IPatient
    {
        ErrorViewModel? Error { get; set; }
        Task<IEnumerable<PatientViewModel>> GetAllPatientsAsync();
        Task<PatientViewModel> AddPatientAsync(PatientViewModel model);
        Task<PatientViewModel?> GetPatientByIdNumberOrContactAsync(string term);
        Task<Patient?> GetPatientByIdAsync(string id);
        Task<ErrorViewModel?> UpdatePatientAsync(PatientViewModel model);
        Task<ErrorViewModel> RemovePatientAsync(string patientId);
    }
}