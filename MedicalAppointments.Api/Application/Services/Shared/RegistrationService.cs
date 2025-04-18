using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using Microsoft.AspNetCore.Identity.UI.Services;
using MedicalAppointments.Api.Migrations;

namespace MedicalAppointments.Api.Application.Services.Shared
{
    public class RegistrationService<T> : IRegistrationService<T> where T : ApplicationUser, new()
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public RegistrationService(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IUserService userService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)userStore;
            _userService = userService;
            _emailSender = emailSender;
        }

        public async Task RegisterAsync(T userData, string password = default!, Hospital hospital = default!)
        {
            var email = userData.Email ?? userData.Contact?.Email ?? hospital.Contact!.Email;

            if (string.IsNullOrWhiteSpace(email))
                return;

            if (await _userStore.FindByNameAsync(email.ToUpper(), CancellationToken.None) != null)
                return;

            var existingUser = !string.IsNullOrEmpty(userData.Id) ? await _userManager.FindByIdAsync(userData.Id) : await _userManager.FindByEmailAsync(email.ToUpper());

            T? user = existingUser as T;
            bool isNewUser = user == null;
            if (existingUser == null)
                user = _userService.CreateUser<T>();

            user!.Title = userData.Title;
            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.CreateDate = DateTime.Now;
            user.IsActive = true;

            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            password = password ?? _userService.GenerateRandomPassword(12);

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

            string role = string.Empty;

            if (result.Succeeded)
            {
                role = typeof(T).Name switch
                {
                    nameof(Patient) => "Patient",
                    nameof(Doctor) => "Doctor",
                    nameof(Admin) => "Admin",
                    nameof(SuperAdmin) => "SuperAdmin",
                    _ => "User"
                };

                await _userManager.AddToRoleAsync(user, role);
            }

            SendCredentials(role, user.Hospital?.Name!, email, password, $"{user.FullName}").Wait();
        }

        public async Task<bool> UpdateUserAsync(T userData)
        {
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null)
                return false;

            user.Title = userData.Title;
            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.Address = userData.Address;
            user.Contact = userData.Contact;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(T userData)
        {
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null)
                return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(T userData, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null)
                return false;
            var result = await _userManager.ChangePasswordAsync(user, user.PasswordHash!, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(T userData, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null)
                return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
        public async Task<bool> ConfirmEmailAsync(T userData)
        {
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null)
                return false;
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task SendCredentials(string role, string hospital, string email, string password, string fullname)
        {
            string body = $@"<p>Dear {fullname},</p>
                          <p>Your {role}'s account at {hospital} has been created successfully.</p>
                          <p><strong>Username:</strong> {email}</p>
                          <p><strong>Temporary Password:</strong> {password}</p>
                          <p>Please note this password is auto generated, no one has it beside you. You can login and change your password whenever you want.</p>
                          <p>If you have any questions, please contact us.</p>
                          <p>Thank you for using our service!</p>
                          <br/><br/>
                          <p>Best regards,</p>
                          <p>The BlckBook Med Team</p>";

            await _emailSender.SendEmailAsync(email, "Your Account Has Been Created", body);
        }
    }
}