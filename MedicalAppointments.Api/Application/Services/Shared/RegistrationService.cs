using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Models;
using Microsoft.AspNetCore.Identity;

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

        public async Task RegisterAsync(T userData)
        {
            var email = userData.Email ?? userData.Contact?.Email;
            if (string.IsNullOrWhiteSpace(email))
                return;

            var existingUser = await _userManager.FindByEmailAsync(email.ToUpper());
            if (existingUser != null)
                return;

            T user = _userService.CreateUser<T>();

            user.Title = userData.Title;
            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.Address = userData.Address;
            user.Contact = userData.Contact;

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var password = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, password);

            if (createUserResult.Succeeded)
            {
                string role = typeof(T).Name switch
                {
                    nameof(Patient) => "Patient",
                    nameof(Doctor) => "Doctor",
                    _ => "User"
                };

                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}