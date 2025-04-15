using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Shared.Controllers
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
        private readonly IAdmin _admin;

        private readonly IRegistrationService<Admin> _registration;

        public HospitalsController(
            IHospital hospital, 
            IHospitalValidation hospitalValidation, 
            IAddress address,
            IContact contact,
            IAdmin admin,
            IRegistrationService<Admin> registration)
        {
            _hospital = hospital;
            _hospitalValidation = hospitalValidation;
            _address = address;
            _contact = contact;
            _admin = admin;
            _registration = registration;
        }

        // GET: api/hospitals
        [HttpGet]
        public async Task<IActionResult> GetHospitals()
        {
            var _hospitals = await _hospital.GetAllHospitalsAsync();

            if(_hospitals == null)
                return NotFound();

            var hospitalVM = _hospitals.Select(h => new HospitalViewModel
            {
                Id = h.Id,
                HospitalName = h.Name,
                ContactDetails = new ContactViewModel 
                { 
                    Id = h.Contact!.Id, 
                    ContactNumber = h.Contact!.ContactNumber, 
                    Email = h.Contact?.Email, 
                    Fax = h.Contact!.Fax 
                },
                AddressDetails = new AddressViewModel 
                { 
                    Id = h.Address!.Id, 
                    Street = h.Address!.Street!, 
                    Suburb = h.Address!.Street!, 
                    City = h.Address!.City!, 
                    Country = h.Address!.Country!,
                    PostalCode = h.Address!.PostalCode! 
                }
            });

            return Ok(hospitalVM);
        }

        // GET: api/hospitals/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospital(int id)
        {
            var hospital = await _hospital.GetHospitalByIdAsync(id);

            if (hospital == null)
                return NotFound();

            var hospitalVM = (new HospitalViewModel
            {
                Id = hospital.Id,
                HospitalName = hospital.Name,
                ContactDetails = new ContactViewModel
                {
                    Id = hospital.Contact!.Id,
                    ContactNumber = hospital.Contact!.ContactNumber,
                    Email = hospital.Contact?.Email,
                    Fax = hospital.Contact!.Fax
                },
                AddressDetails = new AddressViewModel
                {
                    Id = hospital.Address!.Id,
                    Street = hospital.Address!.Street!,
                    Suburb = hospital.Address!.Street!,
                    City = hospital.Address!.City!,
                    Country = hospital.Address!.Country!,
                    PostalCode = hospital.Address!.PostalCode!
                }
            });

            return Ok(hospitalVM);
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
            var admin = new Admin
            {
                Title = "Admin",
                FirstName = hospital.Name,
                LastName = "Admin",
                Hospital = hospital
            };

            admin = await _admin.AddAdminAsync(admin);

            await _registration.RegisterAsync(admin, null!, hospital);
        }
    }
}