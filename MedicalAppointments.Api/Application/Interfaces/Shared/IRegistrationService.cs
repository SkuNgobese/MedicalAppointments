using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Interfaces.Shared
{
    public interface IRegistrationService<T> where T : ApplicationUser
    {
        Task RegisterAsync(T userData, Hospital hospital = default!);
    }
}