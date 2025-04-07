using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Api.Domain.Interfaces.Shared
{
    public interface IUserService
    {
        ApplicationUser CreateUser();
        IUserEmailStore<ApplicationUser> GetEmailStore();
        string GenerateRandomPassword(int length);
    }
}