namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface ICurrentUserHelper
    {
        Task<object?> GetCurrentUserAsync();
        Task<string?> GetUserRoleAsync();
        Task<string> GetCurrentUserId();
    }
}
