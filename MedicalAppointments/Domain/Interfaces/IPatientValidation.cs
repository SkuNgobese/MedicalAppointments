using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IPatientValidation
    {
        bool CanAddPatient(Patient patient, List<Patient> patients);
        bool CanUpdatePatient(Patient patient);
        bool CanRemovePatient(Patient patient);
    }
}
