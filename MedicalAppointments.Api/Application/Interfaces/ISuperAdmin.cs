using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Interfaces
{
    public interface ISuperAdmin
    {
        Task<SuperAdmin> AddAdminAsync(SuperAdmin superAdmin);
        Task<bool> ExistsAsync(string email);
        Task<SuperAdmin?> GetAdminAsync(string email);
    }
}