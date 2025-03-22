using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MedicalAppointments.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MedicalAppointments.Application.ViewModels;
using MedicalAppointments.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MedicalAppointments.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class AppointmentsController : Controller
    {
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IPatientValidation _patientValidation;
        private readonly IAppointment _appointment;
        private readonly IAppointmentValidation _appointmentValidation;

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;

        public AppointmentsController(
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            IUserStore<User> userStore)
        {
            _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            _appointmentValidation = appointmentValidation ?? throw new ArgumentNullException(nameof(appointmentValidation));
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _patientValidation = patientValidation ?? throw new ArgumentNullException(nameof(patientValidation));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _emailStore = GetEmailStore();
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointment.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [HttpGet]
        public IActionResult BookNew()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookNew(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Appointment appointment = await CreateAppointment(model);

            if (_appointmentValidation.CanSchedule(appointment.Date, appointment.Doctor, appointment.Patient))
            {
                await _appointment.BookAppointmentAsync(appointment);

                //Register the patient as a user
                await RegisterPatientAsync(appointment.Patient);

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Could not book appointment at this time.");
            return View(appointment);
        }

        private async Task<Appointment> CreateAppointment(AppointmentViewModel model)
        {
            var doctor = await _doctor.GetDoctorByIdAsync(model.DoctorId) ?? throw new InvalidOperationException("Doctor not found.");

            Patient patient;

            var currentUser = await _userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Unauthorized.");
            
            var hospital = currentUser.Hospital ?? throw new InvalidOperationException("Hospital not found");

            if (!string.IsNullOrEmpty(model.PatientId))
                patient = await _patient.GetPatientByIdAsync(model.PatientId) ?? throw new InvalidOperationException("Patient not found.");
            else
            {
                patient = await CreateNewPatient(model, hospital);
            }

            Appointment appointment = new()
            {
                Date = model.Date,
                Description = model.Description,
                Doctor = doctor,
                Patient = patient ?? throw new InvalidOperationException("Patient cannot be null.")
            };

            return appointment;
        }

        private async Task<Patient> CreateNewPatient(AppointmentViewModel model, Hospital hospital)
        {
            var patient = new Patient()
            {
                Title = model.PatientViewModel.Title,
                FirstName = model.PatientViewModel.FirstName,
                LastName = model.PatientViewModel.LastName,
                IDNumber = model.PatientViewModel.IDNumber,
                Email = model.PatientViewModel.ContactDetails.Email,
                PhoneNumber = model.PatientViewModel.ContactDetails.PhoneNumber,
                IsActive = true,
                CreateDate = DateTime.Now,

                Address = new Address
                {
                    Street = model.PatientViewModel.AddressDetails.Street,
                    City = model.PatientViewModel.AddressDetails.City,
                    Suburb = model.PatientViewModel.AddressDetails.Suburb,
                    PostalCode = model.PatientViewModel.AddressDetails.PostalCode
                },

                Contact = new Contact
                {
                    Fax = model.PatientViewModel.ContactDetails.Fax
                },

                Hospital = hospital
            };

            var patients = await _patient.GetAllPatientsAsync(hospital);

            if (_patientValidation.CanAddPatient(patient, [.. patients]))
                await _patient.AddPatientAsync(patient);

            return patient;
        }

        [HttpGet]
        public async Task<IActionResult> ReAssign(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> ReAssign(int appointmentId, string doctorId)
        {
            Appointment? appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound();

            Doctor? newDoctor = await _doctor.GetDoctorByIdAsync(doctorId);
            if (newDoctor == null) 
                return NotFound();

            if (_appointmentValidation.CanReassign(newDoctor, appointment))
            {
                appointment.Doctor = newDoctor;
                await _appointment.ReAssignAppointmentAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            if (_appointmentValidation.CanCancel(appointment))
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _appointment.CancelAppointmentAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        private async Task RegisterPatientAsync(Patient patient)
        {
            if (patient?.Email == null)
                return;

            var existingUser = await _userStore.FindByNameAsync(patient.Email.ToUpper(), CancellationToken.None);

            if (existingUser != null)
                return;

            User user = CreateUser();

            await _userStore.SetUserNameAsync(user, patient.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, patient.Email, CancellationToken.None);

            string generatedPassword = GenerateRandomPassword(12);
            _ = await _userManager.CreateAsync(user, generatedPassword);
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");
            
            return (IUserEmailStore<User>)_userStore;
        }

        private static string GenerateRandomPassword(int length)
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
