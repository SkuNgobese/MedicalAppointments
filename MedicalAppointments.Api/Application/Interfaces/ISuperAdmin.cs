using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface ISuperAdmin
    {
        Task<SuperAdmin> AddSuperAdminAsync(SuperAdmin superAdmin);
        Task<bool> ExistsAsync(string email);
        Task<SuperAdmin?> GetSuperAdminAsync(string email);
    }
}