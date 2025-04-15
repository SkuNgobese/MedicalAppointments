using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Application.Services;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Api.Infrastructure.Persistence.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Seeds initial roles and the SuperAdmin user.
        /// </summary>
        public static async Task SeedRolesAndSuperAdmin(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var registration = serviceProvider.GetRequiredService<IRegistrationService<SuperAdmin>>();
            var admin = serviceProvider.GetRequiredService<ISuperAdmin>();

            string[] roles = { "SuperAdmin", "Admin", "Doctor", "Patient" };

            // Create roles if they do not exist
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Create the SuperAdmin user if not exists
            var email = configuration["SuperAdmin:Email"]!;
            if (await userManager.FindByEmailAsync(email) is not SuperAdmin)
            {
                SuperAdmin superAdmin = new()
                {
                    Email = email,
                    EmailConfirmed = true,
                    Title = "Mr",
                    FirstName = "Super",
                    LastName = "Admin"
                };

                if (!await admin.ExistsAsync(superAdmin.Email))
                    await admin.AddSuperAdminAsync(superAdmin);

                var superAdminPassword = configuration["SuperAdmin:Password"];

                await registration.RegisterAsync(superAdmin, superAdminPassword!);
            }
        }
    }
}