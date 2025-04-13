using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Api.Models;
using MedicalAppointments.Api.Application.Interfaces;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class HospitalsController : ControllerBase
    {
        private readonly IHospital _hospital;
        private readonly IHospitalValidation _hospitalValidation;
        private readonly IAddress _address;
        private readonly IContact _contact;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        private readonly IUserService _userService;

        public HospitalsController(
            IHospital hospital, 
            IHospitalValidation hospitalValidation, 
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
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
        public async Task<IActionResult> AddHospital([FromBody] Hospital hospital)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        public async Task<IActionResult> UpdateHospital(int id, [FromBody] Hospital hospital)
        {
            if (hospital == null)
                return BadRequest("Invalid hospital data.");
            
            var existingHospital = await _hospital.GetHospitalByIdAsync(id);
            if (existingHospital == null)
                return NotFound($"Hospital with ID {id} not found.");

            // Create and save Address
            existingHospital.Address = new()
            {
                Id = hospital.Address!.Id,
                Street = hospital.Address!.Street,
                City = hospital.Address!.City,
                Suburb = hospital.Address!.Suburb,
                PostalCode = hospital.Address!.PostalCode,
                Country = hospital.Address.Country
            };
            await _address.UpdateAddress(existingHospital.Address);

            // Create and save Contact
            existingHospital.Contact = new()
            {
                Id = hospital.Contact!.Id,
                ContactNumber = hospital.Contact!.ContactNumber,
                Email = hospital.Contact!.Email,
                Fax = hospital.Contact!.Fax
            };
            await _contact.UpdateContact(existingHospital.Contact);

            if (!hospital.Name.Equals(existingHospital.Name))
            {
                var hospitals = await _hospital.GetAllHospitalsAsync();

                if (_hospitalValidation.CanUpdateHospital(hospital, [.. hospitals]))
                {
                    existingHospital.Name = hospital.Name;
                    await _hospital.UpdateHospitalAsync(existingHospital);
                }
            }
            
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

            var existingUser = await _userManager.FindByEmailAsync(hospital.Contact.Email);

            if (existingUser != null)
                return;

            var user = _userService.CreateUser<SysAdmin>();

            user.UserName = hospital.Contact.Email;
            user.Title = "Admin";
            user.FirstName = hospital.Name;
            user.LastName = "Admin";
            user.Hospital = hospital;

            await _userStore.SetUserNameAsync(user, hospital.Contact.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, hospital.Contact.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createUserResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "Admin");
        }
    }
}