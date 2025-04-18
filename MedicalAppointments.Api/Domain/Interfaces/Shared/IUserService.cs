﻿using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Api.Domain.Interfaces.Shared
{
    public interface IUserService
    {
        T CreateUser<T>() where T : ApplicationUser, new();
        IUserEmailStore<ApplicationUser> GetEmailStore();
        string GenerateRandomPassword(int length);
    }
}