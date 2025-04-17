using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,Doctor,Patient")]
    public class PatientsController : ControllerBase
    {
        private readonly IHospital _hospital;
        private readonly IPatient _patient;
        private readonly IPatientValidation _patientValidation;

        private readonly IRegistrationService<Patient> _registration;

        public PatientsController(IHospital hospital,
                                  IPatient patient,
                                  IPatientValidation patientValidation,
                                  IRegistrationService<Patient> registration)
        {
            _hospital = hospital;
            _patient = patient;
            _patientValidation = patientValidation;
            _registration = registration;
        }

        // GET: api/<PatientsController>
        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _patient.GetCurrentUserHospitalPatientsAsync();

            if (patients == null)
                return NotFound();

            var patientVM = patients.Select(p => new PatientViewModel
            {
                Id = p!.Id,
                Title = p.Title!,
                FirstName = p.FirstName!,
                LastName = p.LastName!,
                IDNumber = p.IDNumber!,
                ContactDetails = new ContactViewModel
                {
                    Id = p.Contact!.Id,
                    ContactNumber = p.Contact.ContactNumber,
                    Email = p.Contact.Email,
                    Fax = p.Contact.Fax
                },
                AddressDetails = new AddressViewModel
                {
                    Id = p.Address!.Id,
                    Street = p.Address.Street!,
                    Suburb = p.Address.Suburb!,
                    City = p.Address.City!,
                    PostalCode = p.Address.PostalCode!,
                    Country = p.Address.Country!
                }
            });
            
            return Ok(patientVM);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("patientsearch")]
        public async Task<ActionResult<PatientViewModel>> SearchPatient([FromQuery] string term)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return NotFound("Hospital not found.");

            var patient = await _patient.GetByIdNumberOrContactAsync(hospital, term);

            if (patient is null)
                return NotFound();

            var patientVM = new PatientViewModel
            {
                Id = patient!.Id,
                Title = patient.Title!,
                FirstName = patient.FirstName!,
                LastName = patient.LastName!,
                IDNumber = patient.IDNumber!,
                ContactDetails = new ContactViewModel
                {
                    Id = patient.Contact!.Id,
                    ContactNumber = patient.Contact.ContactNumber,
                    Email = patient.Contact.Email,
                    Fax = patient.Contact.Fax
                },
                AddressDetails = new AddressViewModel
                {
                    Id = patient.Address!.Id,
                    Street = patient.Address.Street!,
                    Suburb = patient.Address.Suburb!,
                    City = patient.Address.City!,
                    PostalCode = patient.Address.PostalCode!,
                    Country = patient.Address.Country!
                }
            };

            return Ok(patientVM);
        }

        // GET: api/<PatientsController>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(string id)
        {
            var patient = await _patient.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return NotFound("Hospital not found.");

            // Validate before adding
            var patients = await _patient.GetAllPatientsAsync(hospital);

            var validationError = _patientValidation.CanAddPatient(patient, [.. patients]);
            if (validationError != null)
                return BadRequest(validationError);

            patient.Hospital = hospital;

            patient = await _patient.AddPatientAsync(patient);

            //Register patient as user
            await _registration.RegisterAsync(patient);

            return Ok(validationError);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPatient = await _patient.GetPatientByIdAsync(id);

            if (existingPatient == null)
                return NotFound();

            var validationError = _patientValidation.CanUpdatePatient(patient);
            if (validationError != null)
                return BadRequest(validationError);

            patient.Id = id;
            await _patient.UpdatePatientAsync(patient);

            return Ok(validationError);
        }
    }
}