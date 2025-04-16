using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IPatientValidation
    {
        ErrorViewModel? CanAddPatient(Patient patient, List<Patient> patients);
        ErrorViewModel? CanUpdatePatient(Patient patient);
        ErrorViewModel? CanRemovePatient(Patient patient);
    }
}