using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Shared.Infrastructure.Persistence.Data
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

            string[] roles = { "SuperAdmin", "Admin", "Doctor", "Patient" };

            // Create roles if they do not exist
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Create the SuperAdmin user if not exists
            var superAdminEmail = configuration["SuperAdmin:Email"]!;
            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdmin == null)
            {
                var newSuperAdmin = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    FirstName = "Super",
                    LastName = "Admin"
                };

                var superAdminPassword = configuration["SuperAdmin:Password"];
                if (!string.IsNullOrWhiteSpace(superAdminPassword))
                {
                    var createUserResult = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);
                    if (createUserResult.Succeeded)
                        await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                }
            }
        }
    }
}
