using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Interfaces
{
    public interface ISysAdmin
    {
        Task<IEnumerable<SysAdmin>> GetAllSysAdminsAsync();
        Task<SysAdmin?> GetAdminAsync(string email);
    }
}