using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface ISuperAdmin
    {
        Task<bool> ExistsAsync(string email);
        Task<SuperAdmin?> GetAdminAsync(string email);
    }
}