using MedicalAppointments.Shared.Domain.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace MedicalAppointments.Shared.Domain.Services.Shared
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
            if (length < 4)
                throw new ArgumentException("Password length must be at least 4 to include all required character types.");

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";
            const string allChars = upper + lower + digits + special;

            var password = new List<char>
            {
                // Ensure at least one character from each category
                upper[RandomNumber(upper.Length)],
                lower[RandomNumber(lower.Length)],
                digits[RandomNumber(digits.Length)],
                special[RandomNumber(special.Length)]
            };

            // Fill the rest with random characters from all sets
            for (int i = password.Count; i < length; i++)
                password.Add(allChars[RandomNumber(allChars.Length)]);

            // Shuffle the password to randomize character positions
            return Shuffle(password);
        }

        private static int RandomNumber(int max)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            return (int)(BitConverter.ToUInt32(bytes, 0) % (uint)max);
        }

        private static string Shuffle(List<char> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            return new string([.. list]);
        }
    }
}
