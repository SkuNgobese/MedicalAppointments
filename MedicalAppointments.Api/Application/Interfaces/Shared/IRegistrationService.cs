using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces.Shared
{
    public interface IRegistrationService<T> where T : ApplicationUser
    {
        Task RegisterAsync(T userData, Hospital hospital = default!);
    }
}