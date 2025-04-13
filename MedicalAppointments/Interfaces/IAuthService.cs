using MedicalAppointments.Api.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Interfaces
{
    public interface IAuthService
    {
        AuthenticationStateProvider AuthenticationStateProvider { get; }
        Task<AuthResponseDto> LoginAsync(LoginDto loginModel);
        Task LogoutAsync();
    }
}