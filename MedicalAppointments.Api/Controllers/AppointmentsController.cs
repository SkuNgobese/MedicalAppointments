using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Models;
using MedicalAppointments.Api.Interfaces;
using MedicalAppointments.Api.Interfaces.Shared;
using MedicalAppointments.Api.Enums;
using MedicalAppointments.Api.Application.Helpers;

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

        private readonly CurrentUserHelper _helpers;

        public AppointmentsController(
            IHospital hospital,
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation,
            IPatientRegistration patientRegistration,
            CurrentUserHelper helpers)
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
            var user = await _helpers.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helpers.GetUserRoleAsync();

            IEnumerable<Appointment> appointments;

            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    appointments = await _appointment.GetAllAppointmentsAsync(doctor!);
                    break;
                case "Patient":
                    var patient = user as Patient;
                    appointments = await _appointment.GetAllAppointmentsAsync(patient!);
                    break;
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    appointments = await _appointment.GetAllAppointmentsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    appointments = await _appointment.GetAllAppointmentsAsync();
                    break;
                default:
                    return Unauthorized("Unauthorized.");
            }

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
        public async Task<IActionResult> BookNew([FromBody] Appointment model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _helpers.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helpers.GetUserRoleAsync();

            Hospital hospital;
            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    hospital = doctor!.Hospital!;
                    break;
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    hospital = admin!.Hospital!;
                    break;
                default:
                    return Unauthorized("Unauthorized.");
            }

            Appointment appointment = await CreateAppointment(model, hospital);

            if (_appointmentValidation.CanSchedule(appointment.Date, appointment.Doctor, appointment.Patient))
            {
                await _appointment.BookAppointmentAsync(appointment);
                //Register the patient as a user
                await _patientRegistration.RegisterPatientAsync(appointment.Patient);
            }

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        private async Task<Appointment> CreateAppointment(Appointment model, Hospital hospital)
        {
            Patient patient;

            if (!string.IsNullOrEmpty(model.Patient.Id))
                patient = await _patient.GetPatientByIdAsync(model.Patient.Id) ?? throw new InvalidOperationException("Patient not found.");
            else
                patient = await AddPatient(model, hospital);

            Appointment appointment = new()
            {
                Date = model.Date,
                Description = model.Description,
                Hospital = hospital,
                Doctor = await _doctor.GetDoctorByIdAsync(model.Doctor.Id) ?? throw new InvalidOperationException("Doctor not found."),
                Patient = patient ?? throw new InvalidOperationException("Patient cannot be null.")
            };

            return appointment;
        }

        private async Task<Patient> AddPatient(Appointment model, Hospital hospital)
        {
            Patient patient = new()
            {
                Title = model.Patient.Title,
                FirstName = model.Patient.FirstName,
                LastName = model.Patient.LastName,
                IDNumber = model.Patient.IDNumber,
                Email = model.Patient.Contact!.Email,
                PhoneNumber = model.Patient.Contact!.ContactNumber,
                IsActive = true,
                CreateDate = DateTime.Now,

                Address = new()
                {
                    Street = model.Patient.Address!.Street,
                    City = model.Patient.Address.City,
                    Suburb = model.Patient.Address.Suburb,
                    PostalCode = model.Patient.Address.PostalCode
                },

                Contact = new()
                {
                    ContactNumber = model.Patient.Contact!.ContactNumber,
                    Email = model.Patient.Contact!.Email,
                    Fax = model.Patient.Contact!.Fax
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
            var user = await _helpers.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helpers.GetUserRoleAsync();

            Hospital hospital;
            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    hospital = doctor!.Hospital!;
                    break;
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    hospital = admin!.Hospital!;
                    break;
                default:
                    return Unauthorized("Unauthorized.");
            }

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
