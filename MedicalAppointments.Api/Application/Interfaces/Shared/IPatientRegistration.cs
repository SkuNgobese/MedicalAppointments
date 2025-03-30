using MedicalAppointments.Api.Domain.Models;

namespace MedicalAppointments.Api.Application.Interfaces.Shared
{
    public interface IPatientRegistration
    {
        Task RegisterPatientAsync(Patient patient);
    }
}
