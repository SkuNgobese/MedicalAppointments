using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Api.Domain.Interfaces.Shared;

namespace MedicalAppointments.Api.Application.Services.Shared
{
    public class RegistrationService<T> : IRegistrationService<T> where T : ApplicationUser, new()
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IUserService _userService;

        public RegistrationService(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IUserService userService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
            _userService = userService;
        }

        public async Task RegisterAsync(T userData, Hospital hospital = default!)
        {
            var email = userData.Contact?.Email ?? hospital.Contact!.Email;
            if (string.IsNullOrWhiteSpace(email))
                return;

            var existingUser = !string.IsNullOrEmpty(userData.Id) ? await _userManager.FindByIdAsync(userData.Id) : await _userManager.FindByEmailAsync(email.ToUpper());

            T? user = existingUser as T;
            bool isNewUser = user == null;
            if (existingUser == null)
                user = _userService.CreateUser<T>();

            user!.Title = userData.Title;
            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var password = _userService.GenerateRandomPassword(12);
            
            IdentityResult result;
            if (isNewUser)
            {
                user.Address = userData.Address;
                user.Contact = userData.Contact;
                result = await _userManager.CreateAsync(user, password);
            }
            else
            {
                _userManager.AddPasswordAsync(user, password).Wait();
                result = await _userManager.UpdateAsync(user);
            }
                

            if (result.Succeeded)
            {
                string role = typeof(T).Name switch
                {
                    nameof(Patient) => "Patient",
                    nameof(Doctor) => "Doctor",
                    nameof(Admin) => "Admin",
                    _ => "User"
                };

                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}