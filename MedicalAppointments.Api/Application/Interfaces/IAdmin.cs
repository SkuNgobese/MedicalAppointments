using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IAdmin
    {
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin?> GetAdminAsync(string email);
        Task<Admin> AddAdminAsync(Admin admin);
        Task RemoveAdminAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);
    }
}