using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace MedicalAppointments.Api.Domain.Services.Shared
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UserService(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
        }

        public T CreateUser<T>() where T : ApplicationUser, new()
        {
            return new T();
        }

        public IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            char[] password = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];

                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                    password[i] = validChars[randomBytes[i] % validChars.Length];
            }

            return new string(password);
        }
    }
}
