using MedicalAppointments.Shared.DTOs.Auth;

namespace MedicalAppointments.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
    }
}