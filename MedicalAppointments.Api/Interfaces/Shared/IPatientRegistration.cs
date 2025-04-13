using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces.Shared
{
    public interface IPatientRegistration
    {
        Task RegisterPatientAsync(Patient patient);
    }
}