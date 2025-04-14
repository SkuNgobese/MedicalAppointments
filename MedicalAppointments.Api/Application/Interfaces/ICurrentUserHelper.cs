namespace MedicalAppointments.Shared.Application.Interfaces
{
    public interface ICurrentUserHelper
    {
        Task<object?> GetCurrentUserAsync();
        Task<string?> GetUserRoleAsync();
    }
}
