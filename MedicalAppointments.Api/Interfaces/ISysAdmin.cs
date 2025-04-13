using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface ISysAdmin
    {
        Task<IEnumerable<SysAdmin>> GetAllSysAdminsAsync();
        Task<SysAdmin?> GetAdminAsync(string email);
        Task RemoveAdminAsync(Hospital hospital);

        Task<bool> ExistsAsync(string email);
    }
}