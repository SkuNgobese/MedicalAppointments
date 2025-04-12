using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Api.Application
{
    public class Helpers
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISysAdmin _sysAdmin;

        public Helpers(IHttpContextAccessor httpContextAccessor, ISysAdmin sysAdmin)
        {
            _httpContextAccessor = httpContextAccessor;
            _sysAdmin = sysAdmin;
        }

        public async Task<SysAdmin?> GetCurrentSysAdminAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(email)) 
                return null;

            return await _sysAdmin.GetAdminAsync(email);
        }
    }
}
