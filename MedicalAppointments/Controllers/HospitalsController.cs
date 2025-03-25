using Microsoft.AspNetCore.Mvc;
using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Application.ViewModels;
using MedicalAppointments.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Domain.Interfaces.Shared;

namespace MedicalAppointments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class HospitalsController : Controller
    {
        private readonly IHospital _hospital;
        private readonly IHospitalValidation _hospitalValidation;
        private readonly IAddress _address;
        private readonly IContact _contact;

        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        private readonly IUserService _userService;

        public HospitalsController(
            IHospital hospital, 
            IHospitalValidation hospitalValidation, 
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IAddress address,
            IContact contact,
            IUserService userService)
        {
            _hospital = hospital;
            _hospitalValidation = hospitalValidation;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userStore = userStore;
            _address = address;
            _contact = contact;
            _userService = userService;
            _emailStore = _userService!.GetEmailStore();
        }

        // GET: api/hospitals
        [HttpGet]
        public async Task<IActionResult> GetHospitals()
        {
            var _hospitals = await _hospital.GetAllHospitalsAsync();

            if(_hospitals == null)
                return NotFound();

            return Ok(_hospitals);
        }

        // GET: api/hospitals/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospital(int id)
        {
            var hospital = await _hospital.GetHospitalByIdAsync(id);
            if (hospital == null)
                return NotFound();

            return Ok(hospital);
        }

        [HttpPost]
        public async Task<IActionResult> AddHospital([FromBody] HospitalViewModel model)
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
                PhoneNumber = model.ContactDetails.PhoneNumber,
                Email = model.ContactDetails.Email,
                Fax = model.ContactDetails.Fax
            };
            contact = await _contact.AddContact(contact);

            // Create Hospital
            Hospital hospital = new()
            {
                Name = model.HospitalName,
                Address = address,
                Contact = contact
            };

            // Validate before adding
            var hospitals = await _hospital.GetAllHospitalsAsync();
            if (!_hospitalValidation.CanAddHospital(hospital, [.. hospitals]))
                return Conflict(new { message = "Hospital already exists or validation failed." });

            hospital = await _hospital.AddHospitalAsync(hospital);

            // Register the Hospital Admin as a user
            await RegisterHospitalAdminAsync(hospital);

            return CreatedAtAction(nameof(GetHospital), new { id = hospital.Id }, hospital);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(int id, [FromBody] HospitalViewModel model)
        {
            if (model == null)
                return BadRequest("Invalid hospital data.");
            
            var existingHospital = await _hospital.GetHospitalByIdAsync(id);
            if (existingHospital == null)
                return NotFound($"Hospital with ID {id} not found.");

            // Create and save Address
            existingHospital.Address = new()
            {
                Street = model.AddressDetails.Street,
                City = model.AddressDetails.City,
                Suburb = model.AddressDetails.Suburb,
                PostalCode = model.AddressDetails.PostalCode
            };
            await _address.UpdateAddress(existingHospital.Address);

            // Create and save Contact
            existingHospital.Contact = new()
            {
                PhoneNumber = model.ContactDetails.PhoneNumber,
                Email = model.ContactDetails.Email,
                Fax = model.ContactDetails.Fax
            };
            await _contact.UpdateContact(existingHospital.Contact);

            // Update existing hospital properties
            existingHospital.Name = model.HospitalName;
            var hospitals = await _hospital.GetAllHospitalsAsync();

            if (_hospitalValidation.CanUpdateHospital(existingHospital, [.. hospitals]))
                await _hospital.UpdateHospitalAsync(existingHospital);

            return Ok(existingHospital);
        }

        // DELETE: api/hospitals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _hospital.GetHospitalByIdAsync(id);
            if (hospital == null) 
                return NotFound();

            if (_hospitalValidation.CanDeleteHospital(hospital))
                await _hospital.RemoveHospitalAsync(hospital);
            
            return NoContent();
        }

        private async Task RegisterHospitalAdminAsync(Hospital hospital)
        {
            if (hospital.Contact?.Email == null)
                return;

            var existingUser = await _userManager.FindByEmailAsync(hospital.Contact.Email.ToUpper());

            if (existingUser != null)
                return;

            User user = _userService.CreateUser();

            user.UserName = hospital.Contact.Email;
            user.Title = "Mr/Mrs";
            user.FirstName = $"{hospital.Name}";
            user.LastName = "Admin";

            await _userStore.SetUserNameAsync(user, hospital.Contact.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, hospital.Contact.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createUserResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
