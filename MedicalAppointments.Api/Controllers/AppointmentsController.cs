using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,Doctor,Patient")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IHospital _hospital;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IPatientValidation _patientValidation;
        private readonly IAppointment _appointment;
        private readonly IAppointmentValidation _appointmentValidation;

        private readonly IRegistrationService<Patient> _registration;

        public AppointmentsController(
            IHospital hospital,
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation,
            IRegistrationService<Patient> registration)
        {
            _hospital = hospital ?? throw new ArgumentNullException(nameof(hospital));
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _patientValidation = patientValidation ?? throw new ArgumentNullException(nameof(patientValidation));
            _registration = registration;
            _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            _appointmentValidation = appointmentValidation ?? throw new ArgumentNullException(nameof(appointmentValidation));
        }

        // GET: api/<AppointmentsController>
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointment.GetCurrentUserHospitalAppointmentsAsync();

            if (appointments == null)
                return NotFound();

            var appointmentVM = appointments.Select(a => new AppointmentViewModel
            {
                Id = a!.Id,
                Date = a.Date,
                Description = a.Description ?? "",
                Status = a.Status,
                PatientViewModel = new PatientViewModel
                {
                    Id = a.Patient.Id,
                    Title = a.Patient!.Title!,
                    FirstName = a.Patient!.FirstName!,
                    LastName = a.Patient!.LastName!,
                    IDNumber = a.Patient!.IDNumber!
                },
                DoctorViewModel = new DoctorViewModel
                {
                    Id = a.Doctor.Id,
                    Title = a.Doctor!.Title!,
                    FirstName = a.Doctor.FirstName!,
                    LastName = a.Doctor.LastName!,
                    Specialization = a.Doctor.Specialization!
                },
                HospitalViewModel = new HospitalViewModel
                {
                    Id = a.Hospital.Id,
                    HospitalName = a.Hospital.Name
                }
            });

            return Ok(appointmentVM);
        }

        [HttpGet("patientsearch")]
        public async Task<ActionResult<PatientViewModel>> SearchPatient([FromQuery] string term)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return NotFound("Hospital not found.");

            var patient = await _patient.GetByIdNumberOrContactAsync(hospital, term);

            if (patient is null)
                return NotFound();

            PatientViewModel patientViewModel = new()
            {
                Id = patient.Id,
                Title = patient.Title!,
                FirstName = patient.FirstName!,
                LastName = patient.LastName!,
                IDNumber = patient.IDNumber!,
            };

            patientViewModel.AppointmentDetails = patient.Appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                Date = a.Date,
                Description = a.Description ?? "Regular checkup",
                Status = a.Status,
                DoctorViewModel = new DoctorViewModel
                {
                    Id = a.Doctor.Id,
                    Title = a.Doctor.Title!,
                    FirstName = a.Doctor.FirstName!,
                    LastName = a.Doctor.LastName!,
                    Specialization = a.Doctor.Specialization!
                },
                HospitalViewModel = new HospitalViewModel
                {
                    Id = a.Hospital.Id,
                    HospitalName = a.Hospital.Name!
                },
                PatientViewModel = patientViewModel
            });

            return Ok(patientViewModel);
        }

        // GET api/<AppointmentsController>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        // POST api/<AppointmentsController>
        [HttpPost]
        public async Task<IActionResult> BookNew([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var  hospital = await _hospital.GetCurrentUserHospitalAsync();
            
            if (hospital == null)
                return BadRequest(ModelState);

            if (_appointmentValidation.CanSchedule(appointment.Date, appointment.Doctor, appointment.Patient))
                await _appointment.BookAppointmentAsync(appointment);
            
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{appointmentId}/reschedule/{newDate}")]
        public async Task<IActionResult> RescheduleAppointment(int appointmentId, DateTime newDate)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest(ModelState);

            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            if (_appointmentValidation.CanReschedule(newDate, appointment))
            {
                appointment.Date = newDate;
                await _appointment.UpdateAppointmentAsync(appointment);

                return Ok(appointment);
            }

            return BadRequest("Reassignment is not allowed.");
        }

        [HttpPut("{appointmentId}/reassign/{doctorId}")]
        public async Task<IActionResult> ReAssignAppointment(int appointmentId, string doctorId)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest(ModelState);

            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var newDoctor = await _doctor.GetDoctorByIdAsync(doctorId, hospital);
            if (newDoctor == null)
                return NotFound("Doctor not found.");

            if (_appointmentValidation.CanReassign(newDoctor, appointment))
            {
                appointment.Doctor = newDoctor;
                await _appointment.UpdateAppointmentAsync(appointment);
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
                appointment.Description = "Cancelled by the patient.";
                await _appointment.UpdateAppointmentAsync(appointment);
            }

            return BadRequest("Cancellation is not allowed for this appointment.");
        }
    }
}