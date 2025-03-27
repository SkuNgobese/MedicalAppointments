using MedicalAppointments.Api.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Api.Domain.Interfaces.Shared
{
    public interface IUserService
    {
        User CreateUser();
        IUserEmailStore<User> GetEmailStore();
        string GenerateRandomPassword(int length);
    }
}
