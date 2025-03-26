using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Domain.Interfaces.Shared;
using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalAppointments.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicalAppointments.Controllers
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

        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        private readonly IUserService _userService;
        private Task<User?> _currentUser;

        public DoctorsController(
            IHospital hospital,
            IDoctor doctor,
            IDoctorValidation doctorValidation,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IAddress address,
            IContact contact,
            IUserService userService)
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

            _currentUser = _userManager.GetUserAsync(User);
        }

        // GET: api/<DoctorsController>
        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var currentUser = await _currentUser ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = currentUser.Hospital ?? throw new InvalidOperationException("Hospital not found");

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
            var currentUser = await _currentUser ?? throw new InvalidOperationException("Unauthorized.");

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
                PhoneNumber = model.ContactDetails.PhoneNumber,
                Email = model.ContactDetails.Email,
                Fax = model.ContactDetails.Fax
            };
            contact = await _contact.AddContact(contact);

            var hospital = currentUser.Hospital ?? throw new InvalidOperationException("Hospital not found");

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
                PhoneNumber = model.ContactDetails.PhoneNumber,
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
