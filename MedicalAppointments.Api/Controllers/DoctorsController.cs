using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Api.Models;
using MedicalAppointments.Api.ViewModels;
using MedicalAppointments.Api.Interfaces;
using MedicalAppointments.Api.Application.Helpers;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DoctorsController : ControllerBase
    {
        private readonly IHospital _hospital;
        private readonly IDoctor _doctor;
        private readonly IDoctorValidation _doctorValidation;
        private readonly IAddress _address;
        private readonly IContact _contact;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        private readonly IUserService _userService;

        private readonly CurrentUserHelper _helpers;

        public DoctorsController(
            IHospital hospital,
            IDoctor doctor,
            IDoctorValidation doctorValidation,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IAddress address,
            IContact contact,
            IUserService userService,
            CurrentUserHelper helpers)
        {
            _hospital = hospital;
            _doctor = doctor;
            _doctorValidation = doctorValidation;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userStore = userStore;
            _address = address;
            _contact = contact;
            _userService = userService;
            _emailStore = _userService!.GetEmailStore();
            _helpers = helpers;
        }

        // GET: api/<DoctorsController>
        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var user = await _helpers.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helpers.GetUserRoleAsync();

            IEnumerable<Doctor> doctors;

            switch (role)
            {
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    doctors = await _doctor.GetAllDoctorsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    doctors = await _doctor.GetAllDoctorsAsync();
                    break;
                default:
                    return Unauthorized("Unauthorized.");
            }

            if (doctors == null)
                return NotFound();

            return Ok(doctors);
        }

        // GET: api/<DoctorsController>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(string id)
        {
            var doctor = await _doctor.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sysAdmin = await _helpers.GetCurrentUserAsync() as SysAdmin ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            // Validate before adding
            var doctors = await _doctor.GetAllDoctorsAsync(hospital);
            if (_doctorValidation.CanAdd(doctor, [.. doctors]))
            {
                doctor.Hospital = hospital;
                doctor.IsActive = true;

                doctor = await _doctor.EnrollDoctorAsync(doctor);

                await RegisterDoctorAsUserAsync(doctor);
            }

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }

        // PUT api/<DoctorsController>/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] DoctorViewModel model)
        {
            if (model == null)
                return BadRequest("Invalid doctor data.");

            var doctor = await _doctor.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            // Create and save Address
            doctor.Address = new()
            {
                Street = model.AddressDetails.Street,
                City = model.AddressDetails.City,
                Suburb = model.AddressDetails.Suburb,
                PostalCode = model.AddressDetails.PostalCode
            };
            await _address.UpdateAddress(doctor.Address);

            // Create and save Contact
            doctor.Contact = new()
            {
                ContactNumber = model.ContactDetails.ContactNumber,
                Email = model.ContactDetails.Email,
                Fax = model.ContactDetails.Fax
            };
            await _contact.UpdateContact(doctor.Contact);

            await _doctor.UpdateDoctorAsync(doctor);

            return Ok(doctor);
        }

        // DELETE api/<DoctorsController>/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _doctor.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            if (_doctorValidation.CanRemove(doctor))
                await _doctor.RemoveDoctorAsync(doctor);

            return NoContent();
        }

        private async Task RegisterDoctorAsUserAsync(Doctor doctor)
        {
            if (doctor.Contact?.Email == null)
                throw new NullReferenceException("Doctor's email is required.");

            var existingUser = await _userManager.FindByEmailAsync(doctor.Contact.Email);

            if (existingUser != null)
                return;

            var user = _userService.CreateUser<Doctor>();
            user.UserName = doctor.Contact.Email;
            user.Hospital = doctor.Hospital;

            await _userStore.SetUserNameAsync(user, doctor.Contact.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, doctor.Contact.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createUserResult.Succeeded)
                await _userManager.AddToRoleAsync(doctor, "Doctor");
        }
    }
}
