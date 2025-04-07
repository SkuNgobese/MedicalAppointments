using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Interfaces.Shared;
using MedicalAppointments.Shared.ViewModels;
using MedicalAppointments.Shared.Enums;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,Doctor")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IPatientValidation _patientValidation;
        private readonly IAppointment _appointment;
        private readonly IAppointmentValidation _appointmentValidation;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientRegistration _patientRegistration;
        private readonly Task<ApplicationUser?> _currentUser;
        private readonly Hospital _hospital;

        public AppointmentsController(
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation,
            UserManager<ApplicationUser> userManager,
            IPatientRegistration patientRegistration)
        {
            _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            _appointmentValidation = appointmentValidation ?? throw new ArgumentNullException(nameof(appointmentValidation));
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _patientValidation = patientValidation ?? throw new ArgumentNullException(nameof(patientValidation));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _patientRegistration = patientRegistration;

            _currentUser = _userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Unauthorized.");
            _hospital = _currentUser.Result?.Hospital ?? throw new InvalidOperationException("Hospital not found");
        }

        // GET: api/<AppointmentController>
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointment.GetAllAppointmentsAsync(_hospital);

            if (appointments == null)
                return NotFound();

            return Ok(appointments);
        }

        // GET api/<AppointmentController>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        // POST api/<AppointmentController>
        [HttpPost]
        public async Task<IActionResult> BookNew([FromBody] AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Appointment appointment = await CreateAppointment(model);

            if (_appointmentValidation.CanSchedule(appointment.Date, appointment.Doctor, appointment.Patient))
            {
                await _appointment.BookAppointmentAsync(appointment);
                //Register the patient as a user
                await _patientRegistration.RegisterPatientAsync(appointment.Patient);
            }

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        private async Task<Appointment> CreateAppointment(AppointmentViewModel model)
        {
            Patient patient;

            if (!string.IsNullOrEmpty(model.PatientId))
                patient = await _patient.GetPatientByIdAsync(model.PatientId) ?? throw new InvalidOperationException("Patient not found.");
            else
                patient = await CreateNewPatient(model);

            Appointment appointment = new()
            {
                Date = model.Date,
                Description = model.Description,
                Hospital = _hospital,
                Doctor = await _doctor.GetDoctorByIdAsync(model.DoctorId) ?? throw new InvalidOperationException("Doctor not found."),
                Patient = patient ?? throw new InvalidOperationException("Patient cannot be null.")
            };

            return appointment;
        }

        private async Task<Patient> CreateNewPatient(AppointmentViewModel model)
        {
            Patient patient = new()
            {
                Title = model.PatientViewModel.Title,
                FirstName = model.PatientViewModel.FirstName,
                LastName = model.PatientViewModel.LastName,
                IDNumber = model.PatientViewModel.IDNumber,
                Email = model.PatientViewModel.ContactDetails.Email,
                PhoneNumber = model.PatientViewModel.ContactDetails.ContactNumber,
                IsActive = true,
                CreateDate = DateTime.Now,

                Address = new()
                {
                    Street = model.PatientViewModel.AddressDetails.Street,
                    City = model.PatientViewModel.AddressDetails.City,
                    Suburb = model.PatientViewModel.AddressDetails.Suburb,
                    PostalCode = model.PatientViewModel.AddressDetails.PostalCode
                },

                Contact = new()
                {
                    ContactNumber = model.PatientViewModel.ContactDetails.ContactNumber,
                    Email = model.PatientViewModel.ContactDetails.Email,
                    Fax = model.PatientViewModel.ContactDetails.Fax
                },

                Hospital = _hospital
            };

            var patients = await _patient.GetAllPatientsAsync(_hospital);

            if (_patientValidation.CanAddPatient(patient, [.. patients]))
                await _patient.AddPatientAsync(patient);

            return patient;
        }

        [HttpPut("{appointmentId}/reassign/{doctorId}")]
        public async Task<IActionResult> ReAssignAppointment(int appointmentId, string doctorId)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var newDoctor = await _doctor.GetDoctorByIdAsync(doctorId, _hospital);
            if (newDoctor == null)
                return NotFound("Doctor not found.");

            if (_appointmentValidation.CanReassign(newDoctor, appointment))
            {
                appointment.Doctor = newDoctor;
                await _appointment.ReAssignAppointmentAsync(appointment);
                return Ok(appointment);
            }

            return BadRequest("Reassignment is not allowed.");
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found.");

            if (_appointmentValidation.CanCancel(appointment))
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _appointment.CancelAppointmentAsync(appointment);
            }

            return BadRequest("Cancellation is not allowed for this appointment.");
        }
    }
}
