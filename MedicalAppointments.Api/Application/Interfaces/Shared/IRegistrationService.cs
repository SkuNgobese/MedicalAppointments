using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Interfaces.Shared
{
    public interface IRegistrationService<T> where T : ApplicationUser
    {
        Task RegisterAsync(T userData);
    }
}