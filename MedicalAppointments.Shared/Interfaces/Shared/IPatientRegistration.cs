using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Interfaces.Shared
{
    public interface IPatientRegistration
    {
        Task RegisterPatientAsync(Patient patient);
    }
}