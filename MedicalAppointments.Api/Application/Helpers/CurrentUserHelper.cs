﻿using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Helpers
{
    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetCurrentUserId()
        {
            var user = await GetCurrentUserAsync();
            return user == null
                ? throw new InvalidOperationException("Unauthorized.")
                : user switch
            {
                SuperAdmin superAdmin => superAdmin.Id,
                Admin admin => admin.Id,
                Doctor doctor => doctor.Id,
                Patient patient => patient.Id,
                _ => throw new InvalidOperationException("Unauthorized.")
            };
        }

        public async Task<object?> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var superAdmin = _serviceProvider.GetRequiredService<ISuperAdmin>();
            if (await superAdmin.ExistsAsync(email))
                return await superAdmin.GetSuperAdminAsync(email);

            var sysAdmin = _serviceProvider.GetRequiredService<IAdmin>();
            if (await sysAdmin.ExistsAsync(email))
                return await sysAdmin.GetAdminAsync(email);

            var doctor = _serviceProvider.GetRequiredService<IDoctor>();
            if (await doctor.ExistsAsync(email))
                return await doctor.GetDoctorAsync(email);

            var patient = _serviceProvider.GetRequiredService<IPatient>();
            if (await patient.ExistsAsync(email))
                return await patient.GetPatientAsync(email);

            return null;
        }

        public async Task<string?> GetUserRoleAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var superAdmin = _serviceProvider.GetRequiredService<ISuperAdmin>();
            if (await superAdmin.ExistsAsync(email))
                return "SuperAdmin";

            var sysAdmin = _serviceProvider.GetRequiredService<IAdmin>();
            if (await sysAdmin.ExistsAsync(email))
                return "Admin";

            var doctor = _serviceProvider.GetRequiredService<IDoctor>();
            if (await doctor.ExistsAsync(email))
                return "Doctor";

            var patient = _serviceProvider.GetRequiredService<IPatient>();
            if (await patient.ExistsAsync(email))
                return "Patient";

            return null;
        }
    }
}