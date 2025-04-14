using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Shared.Domain.Interfaces;
using MedicalAppointments.Shared.Domain.Interfaces.Shared;
using MedicalAppointments.Shared.ViewModels;
using MedicalAppointments.Shared.Application.Helpers;
using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Application.Interfaces.Shared;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Controllers
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

        private readonly IRegistrationService<Doctor> _registration;

        public DoctorsController(
            IHospital hospital,
            IDoctor doctor,
            IDoctorValidation doctorValidation,
            IAddress address,
            IContact contact,
            IRegistrationService<Doctor> registration)
        {
            _hospital = hospital;
            _doctor = doctor;
            _doctorValidation = doctorValidation;
            _address = address;
            _contact = contact;
            _registration = registration;
        }

        // GET: api/<DoctorsController>
        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _doctor.GetCurrentUserHospitalDoctorsAsync();

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

            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest(ModelState);

            // Validate before adding
            var doctors = await _doctor.GetAllDoctorsAsync(hospital);
            if (_doctorValidation.CanAdd(doctor, [.. doctors]))
            {
                doctor.Hospital = hospital;
                doctor.IsActive = true;

                doctor = await _doctor.EnrollDoctorAsync(doctor);

                //Register doctor as user
                await _registration.RegisterAsync(doctor);
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
    }
}
