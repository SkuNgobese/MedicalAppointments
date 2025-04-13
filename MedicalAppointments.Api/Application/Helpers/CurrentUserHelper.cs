using MedicalAppointments.Api.Interfaces;

namespace MedicalAppointments.Api.Application.Helpers
{
    public class CurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISysAdmin _sysAdmin;
        private readonly ISuperAdmin _superAdmin;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;

        public CurrentUserHelper(
            IHttpContextAccessor httpContextAccessor,
            ISysAdmin sysAdmin,
            ISuperAdmin superAdmin,
            IDoctor doctor,
            IPatient patient)
        {
            _httpContextAccessor = httpContextAccessor;
            _sysAdmin = sysAdmin;
            _superAdmin = superAdmin;
            _doctor = doctor;
            _patient = patient;
        }

        public async Task<object?> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) 
                return null;

            if (await _superAdmin.ExistsAsync(email))
                return await _superAdmin.GetAdminAsync(email);

            if (await _sysAdmin.ExistsAsync(email))
                return await _sysAdmin.GetAdminAsync(email);

            if (await _doctor.ExistsAsync(email))
                return await _doctor.GetDoctorAsync(email);

            if (await _patient.ExistsAsync(email))
                return await _patient.GetPatientAsync(email);

            return null;
        }

        public async Task<string?> GetUserRoleAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) 
                return null;

            if (await _superAdmin.ExistsAsync(email)) 
                return "SuperAdmin";
            if (await _sysAdmin.ExistsAsync(email)) 
                return "SysAdmin";
            if (await _doctor.ExistsAsync(email)) 
                return "Doctor";
            if (await _patient.ExistsAsync(email)) 
                return "Patient";

            return null;
        }
    }
}
