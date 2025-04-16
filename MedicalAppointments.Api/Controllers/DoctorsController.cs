using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,Doctor,Patient")]
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

            var doctorVM = doctors.Select(d => new DoctorViewModel
            {
                Id = d!.Id,
                Title = d.Title!,
                FirstName = d.FirstName!,
                LastName = d.LastName!,
                Specialization = d.Specialization!,
                IDNumber = d.IDNumber!,
                HireDate = d.HireDate ?? DateTime.Now,
                ContactDetails = d.Contact != null ? new ContactViewModel 
                {
                    Id = d.Contact!.Id,
                    ContactNumber = d.Contact!.ContactNumber, 
                    Email = d.Contact?.Email, 
                    Fax = d.Contact!.Fax
                } : null,
                AddressDetails = d.Address != null ? new AddressViewModel 
                {
                    Id = d.Address!.Id,
                    Street = d.Address!.Street!, 
                    Suburb = d.Address!.Street!, 
                    City = d.Address!.City!, 
                    Country = d.Address!.Country!, 
                    PostalCode = d.Address!.PostalCode!
                } : null
            });

            return Ok(doctorVM);
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest(ModelState);

            // Validate before adding
            var doctors = await _doctor.GetAllDoctorsAsync(hospital);
            var validationError = _doctorValidation.CanAdd(doctor, [.. doctors]);
            if (validationError != null)
                return BadRequest(validationError);

            doctor.Hospital = hospital;
            doctor = await _doctor.EnrollDoctorAsync(doctor);

            //Register doctor as user
            await _registration.RegisterAsync(doctor);

            return Ok(doctor);
        }

        // PUT api/<DoctorsController>/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] Doctor model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest("Invalid doctor data.");

            var doctor = await _doctor.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            // Update and save Address
            doctor.Address = model.Address;

            await _address.UpdateAddress(doctor.Address!);

            // Update and save Contact
            doctor.Contact = model.Contact;

            await _contact.UpdateContact(doctor.Contact!);

            await _doctor.UpdateDoctorAsync(doctor);

            return Ok(doctor);
        }

        // DELETE api/<DoctorsController>/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _doctor.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            var validationError = _doctorValidation.CanRemove(doctor);
            if (validationError != null)
                return BadRequest(validationError);
            
            await _doctor.RemoveDoctorAsync(doctor);

            return Ok(validationError);
        }
    }
}