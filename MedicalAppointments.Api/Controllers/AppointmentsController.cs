using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Enums;
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
        private readonly IAppointment _appointment;
        private readonly IAppointmentValidation _appointmentValidation;

        private readonly ICurrentUserHelper _helper;

        public AppointmentsController(
            IHospital hospital,
            IDoctor doctor,
            IPatient patient,
            IPatientValidation patientValidation,
            IAppointment appointment,
            IAppointmentValidation appointmentValidation, 
            ICurrentUserHelper helper)
        {
            _hospital = hospital ?? throw new ArgumentNullException(nameof(hospital));
            _doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            _appointmentValidation = appointmentValidation ?? throw new ArgumentNullException(nameof(appointmentValidation));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
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

                PatientViewModel = a.Patient != null ? new PatientViewModel
                {
                    Id = a.Patient.Id,
                    Title = a.Patient.Title ?? "",
                    FirstName = a.Patient.FirstName ?? "",
                    LastName = a.Patient.LastName ?? "",
                    IDNumber = a.Patient.IDNumber ?? "",
                    ContactDetails = a.Patient.Contact != null ? new ContactViewModel
                    {
                        ContactNumber = a.Patient.Contact.ContactNumber ?? "",
                        Email = a.Patient.Contact.Email ?? ""
                    } : null
                } : null,

                DoctorViewModel = a.Doctor != null ? new DoctorViewModel
                {
                    Id = a.Doctor.Id,
                    Title = a.Doctor.Title ?? "",
                    FirstName = a.Doctor.FirstName ?? "",
                    LastName = a.Doctor.LastName ?? "",
                    Specialization = a.Doctor.Specialization ?? ""
                } : null,

                HospitalViewModel = a.Hospital != null ? new HospitalViewModel
                {
                    Id = a.Hospital.Id,
                    HospitalName = a.Hospital.Name ?? ""
                } : null
            });

            return Ok(appointmentVM);
        }

        // GET api/<AppointmentsController>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var appointment = await _appointment.GetAppointmentByIdAsync(id);

            if (appointment == null)
                return NotFound();

            var appointmentVM = new AppointmentViewModel
            {
                Id = appointment!.Id,
                Date = appointment.Date,
                Description = appointment.Description ?? "",
                Status = appointment.Status,

                PatientViewModel = appointment.Patient != null ? new PatientViewModel
                {
                    Id = appointment.Patient.Id,
                    Title = appointment.Patient.Title ?? "",
                    FirstName = appointment.Patient.FirstName ?? "",
                    LastName = appointment.Patient.LastName ?? "",
                    IDNumber = appointment.Patient.IDNumber ?? "",
                    ContactDetails = appointment.Patient.Contact != null ? new ContactViewModel
                    {
                        ContactNumber = appointment.Patient.Contact.ContactNumber ?? "",
                        Email = appointment.Patient.Contact.Email ?? ""
                    } : null
                } : null,

                DoctorViewModel = appointment.Doctor != null ? new DoctorViewModel
                {
                    Id = appointment.Doctor.Id,
                    Title = appointment.Doctor.Title ?? "",
                    FirstName = appointment.Doctor.FirstName ?? "",
                    LastName = appointment.Doctor.LastName ?? "",
                    Specialization = appointment.Doctor.Specialization ?? ""
                } : null,

                HospitalViewModel = appointment.Hospital != null ? new HospitalViewModel
                {
                    Id = appointment.Hospital.Id,
                    HospitalName = appointment.Hospital.Name ?? ""
                } : null
            };

            return Ok(appointmentVM);
        }

        // POST api/<AppointmentsController>
        [HttpPost]
        public async Task<IActionResult> BookNew([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var  hospital = await _hospital.GetCurrentUserHospitalAsync();
            
            if (hospital == null)
                return BadRequest("Hospital not found.");

            var doctor = await _doctor.GetDoctorByIdAsync(appointment.Doctor!.Id, hospital);

            if (doctor == null)
                return BadRequest("Doctor not found.");

            var patient = await _patient.GetPatientByIdAsync(appointment.Patient!.Id);

            if (patient == null)
                return BadRequest("Patient not found.");

            appointment.Hospital = hospital;
            appointment.Doctor = doctor;
            appointment.Patient = patient;

            var validationError = _appointmentValidation.CanSchedule(appointment.Date, appointment.Doctor, appointment.Patient);
            if (validationError != null)
                return BadRequest(validationError);

            appointment.CreatedDate = DateTime.Now;
            appointment.CreatedBy = await _helper.GetCurrentUserId();
            await _appointment.AddAppointmentAsync(appointment);

            return Ok(validationError);
        }

        [HttpPut("{appointmentId}/reschedule/{newDate}")]
        public async Task<IActionResult> RescheduleAppointment(int appointmentId, DateTime newDate)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest("Unauthorized");

            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var validationError = _appointmentValidation.CanReschedule(newDate, appointment);
            if (validationError != null)
                return BadRequest(validationError);

            appointment.Date = newDate;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = await _helper.GetCurrentUserId();
            await _appointment.UpdateAppointmentAsync(appointment);

            return Ok(validationError);
        }

        [HttpPut("{appointmentId}/reassign/{doctorId}")]
        public async Task<IActionResult> ReAssignAppointment(int appointmentId, string doctorId)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest("Unauthorized");

            var appointment = await _appointment.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var newDoctor = await _doctor.GetDoctorByIdAsync(doctorId, hospital);
            if (newDoctor == null)
                return NotFound("Doctor not found.");

            var validationError = _appointmentValidation.CanReassign(newDoctor, appointment);
            if (validationError != null)
                return BadRequest(validationError);

            appointment.Doctor = newDoctor;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = await _helper.GetCurrentUserId();
            await _appointment.UpdateAppointmentAsync(appointment);

            return Ok(validationError);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var hospital = await _hospital.GetCurrentUserHospitalAsync();

            if (hospital == null)
                return BadRequest("Unauthorized");

            var appointment = await _appointment.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var validationError = _appointmentValidation.CanCancel(appointment);
            if (validationError != null)
                return BadRequest(validationError);

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = await _helper.GetCurrentUserId();
            appointment.Description = "Cancelled by the patient.";
            await _appointment.UpdateAppointmentAsync(appointment);
            
            return Ok(appointment);
        }
    }
}