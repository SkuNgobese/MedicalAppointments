using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Interfaces.Shared;
using MedicalAppointments.Shared.ViewModels;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Api.Application;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,Doctor")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IHospital _hospital;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IPatientValidation _patientValidation;
        private readonly IAppointment _appointment;
        private readonly IAppointmentValidation _appointmentValidation;

        private readonly IPatientRegistration _patientRegistration;

        private readonly Helpers _helpers;

        public AppointmentsController(
            IHospital hospital,
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation,
            IPatientRegistration patientRegistration,
            Helpers helpers)
        {
            _hospital = hospital ?? throw new ArgumentNullException(nameof(hospital));
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _patientValidation = patientValidation ?? throw new ArgumentNullException(nameof(patientValidation));
            _patientRegistration = patientRegistration;
            _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            _appointmentValidation = appointmentValidation ?? throw new ArgumentNullException(nameof(appointmentValidation));
            _helpers = helpers ?? throw new ArgumentNullException(nameof(helpers));
        }

        // GET: api/<AppointmentController>
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var sysAdmin = await _helpers.GetCurrentSysAdminAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            var appointments = await _appointment.GetAllAppointmentsAsync(hospital);

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
            var sysAdmin = await _helpers.GetCurrentSysAdminAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            Patient patient;

            if (!string.IsNullOrEmpty(model.PatientId))
                patient = await _patient.GetPatientByIdAsync(model.PatientId) ?? throw new InvalidOperationException("Patient not found.");
            else
                patient = await CreateNewPatient(model, hospital);

            Appointment appointment = new()
            {
                Date = model.Date,
                Description = model.Description,
                Hospital = hospital,
                Doctor = await _doctor.GetDoctorByIdAsync(model.DoctorId) ?? throw new InvalidOperationException("Doctor not found."),
                Patient = patient ?? throw new InvalidOperationException("Patient cannot be null.")
            };

            return appointment;
        }

        private async Task<Patient> CreateNewPatient(AppointmentViewModel model, Hospital hospital)
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

                Hospital = hospital
            };

            var patients = await _patient.GetAllPatientsAsync(hospital);

            if (_patientValidation.CanAddPatient(patient, [.. patients]))
                await _patient.AddPatientAsync(patient);

            return patient;
        }

        [HttpPut("{appointmentId}/reassign/{doctorId}")]
        public async Task<IActionResult> ReAssignAppointment(int appointmentId, string doctorId)
        {
            var sysAdmin = await _helpers.GetCurrentSysAdminAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var hospital = await _hospital.GetHospitalByIdAsync(sysAdmin!.Hospital!.Id) ?? throw new InvalidOperationException("Hospital not found");

            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var newDoctor = await _doctor.GetDoctorByIdAsync(doctorId, hospital);
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
