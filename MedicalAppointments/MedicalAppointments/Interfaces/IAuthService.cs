using MedicalAppointments.Shared.DTOs.Auth;

namespace MedicalAppointments.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginModel);
        Task LogoutAsync();
    }
}