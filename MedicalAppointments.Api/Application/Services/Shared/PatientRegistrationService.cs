﻿using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Shared.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Api.Application.Services.Shared
{
    public class PatientRegistrationService : IPatientRegistration
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IUserService _userService;

        public PatientRegistrationService(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IUserService userService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = _userService!.GetEmailStore();
            _userService = userService;
        }

        public async Task RegisterPatientAsync(Patient patient)
        {
            if (patient?.Email == null)
                return;

            var existingUser = await _userManager.FindByEmailAsync(patient.Email.ToUpper());

            if (existingUser != null)
                return;

            ApplicationUser user = _userService.CreateUser();

            user.UserName = patient.Email;
            user.Title = patient.Title;
            user.FirstName = patient.FirstName;
            user.LastName = patient.LastName;
            user.Address = patient.Address;
            user.Contact = patient.Contact;
            user.Hospital = patient.Hospital;

            await _userStore.SetUserNameAsync(user, patient.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, patient.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createUserResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "Patient");
        }
    }
}