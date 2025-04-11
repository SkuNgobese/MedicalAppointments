using MedicalAppointments.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Client.Interfaces
{
    public interface IAuthService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        Task<AuthResponseDto> LoginAsync(LoginDto loginModel);
        Task LogoutAsync();
    }
}