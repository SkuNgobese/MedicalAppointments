using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.ViewModels;

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
                return BadRequest("Invalid hospital ID.");

            var hospitalVM = new HospitalViewModel
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
            };

            return Ok(hospitalVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddHospital([FromBody] Hospital hospital)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate before adding
            var hospitals = await _hospital.GetAllHospitalsAsync();

            var validationError = _hospitalValidation.CanAddHospital(hospital, [.. hospitals]);
            if (validationError != null)
                return BadRequest(validationError);

            hospital = await _hospital.AddHospitalAsync(hospital);

            // Register the Hospital Admin as a user
            await RegisterHospitalAdminAsync(hospital);

            return Ok(validationError);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(int id, [FromBody] HospitalViewModel model)
        {
            var hospital = await _hospital.GetHospitalByIdAsync(id);

            if (model == null)
                return BadRequest("Invalid hospital data.");
            
            var hospitals = await _hospital.GetAllHospitalsAsync();

            var validationError = _hospitalValidation.CanUpdateHospital(hospital!, [.. hospitals]);
            if (validationError != null)
                return BadRequest(validationError);

            hospital!.Name = model.HospitalName;

            hospital.Address!.Street = model.AddressDetails!.Street;
            hospital.Address.Suburb = model.AddressDetails.Suburb;
            hospital.Address.City = model.AddressDetails.City;
            hospital.Address.PostalCode = model.AddressDetails.PostalCode;
            hospital.Address.Country = model.AddressDetails.Country;

            hospital.Contact!.ContactNumber = model.ContactDetails!.ContactNumber;
            hospital.Contact.Fax = model.ContactDetails!.Fax;

            await _hospital.UpdateHospitalAsync(hospital);
            
            return Ok(validationError);
        }

        // DELETE: api/hospitals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _hospital.GetHospitalByIdAsync(id);
            if (hospital == null)
                return NotFound($"Hospital with ID {id} not found.");

            var validationError = _hospitalValidation.CanDeleteHospital(hospital);
            if (validationError != null)
                return BadRequest(validationError);

            await _hospital.RemoveHospitalAsync(hospital);
            
            return Ok(validationError);
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