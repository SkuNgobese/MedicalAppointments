using MedicalAppointments.Api.Domain.Models;

namespace MedicalAppointments.Application.Interfaces.Shared
{
    public interface IPatientRegistration
    {
        Task RegisterPatientAsync(Patient patient);
    }
}
