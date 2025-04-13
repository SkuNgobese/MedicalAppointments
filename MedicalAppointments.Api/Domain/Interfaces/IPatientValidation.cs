using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IPatientValidation
    {
        bool CanAddPatient(Patient patient, List<Patient> patients);
        bool CanUpdatePatient(Patient patient);
        bool CanRemovePatient(Patient patient);
    }
}