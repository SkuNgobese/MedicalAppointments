using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Domain.Interfaces.Shared
{
    public interface IUserService
    {
        User CreateUser();
        IUserEmailStore<User> GetEmailStore();
        string GenerateRandomPassword(int length);
    }
}
