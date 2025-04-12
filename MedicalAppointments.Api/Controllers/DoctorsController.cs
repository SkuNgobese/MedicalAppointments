using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using MedicalAppointments.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using MedicalAppointments.Api.Application;

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

        private readonly Helpers _helpers;

        public DoctorsController(
            IHospital hospital,
            IDoctor doctor,
            IDoctorValidation doctorValidation,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IAddress address,
            IContact contact,
            IUserService userService,
            Helpers helpers)
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
            var sysAdmin = await _helpers.GetCurrentSysAdminAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            var _doctors = await _doctor.GetAllDoctorsAsync(hospital);

            if (_doctors == null)
                return NotFound();

            return Ok(_doctors);
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
        public async Task<IActionResult> AddDoctor([FromBody] DoctorViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create and save Address
            Address address = new()
            {
                Street = model.AddressDetails.Street,
                City = model.AddressDetails.City,
                Suburb = model.AddressDetails.Suburb,
                PostalCode = model.AddressDetails.PostalCode
            };
            address = await _address.AddAddress(address);

            // Create and save Contact
            Contact contact = new()
            {
                ContactNumber = model.ContactDetails.ContactNumber,
                Email = model.ContactDetails.Email,
                Fax = model.ContactDetails.Fax
            };
            contact = await _contact.AddContact(contact);

            var sysAdmin = await _helpers.GetCurrentSysAdminAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            Doctor doctor = new()
            {
                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IDNumber = model.IDNumber,
                Specialization = model.Specialization,
                HireDate = DateTime.Now,
                IsActive = true,
                Hospital = hospital,
                Address = address,
                Contact = contact
            };

            // Validate before adding
            var doctors = await _doctor.GetAllDoctorsAsync(hospital);
            if (_doctorValidation.CanAdd(doctor, [.. doctors]))
                await AddDoctorAndRegisterAsUserAsync(doctor);

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

        private async Task<Doctor?> AddDoctorAndRegisterAsUserAsync(Doctor doctor)
        {
            if (doctor.Contact?.Email == null)
                throw new NullReferenceException("Doctor's email is required.");

            var existingUser = await _userManager.FindByEmailAsync(doctor.Contact.Email);

            if (existingUser != null)
                return existingUser as Doctor;

            await _userStore.SetUserNameAsync(doctor, doctor.Contact.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(doctor, doctor.Contact.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(doctor, generatedPassword);

            if (createUserResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(doctor, "Doctor");
                return doctor;
            }

            throw new InvalidOperationException("Doctor could not be added.");
        }
    }
}
