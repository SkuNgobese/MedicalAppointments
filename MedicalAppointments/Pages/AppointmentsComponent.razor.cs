using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Pages
{
    public partial class AppointmentsComponent
    {
        [Inject]
        public IAppointment? Appointment { get; set; }

        [Inject]
        public IPatient? Patient { get; set; }

        [Inject]
        public IDoctor? Doctor { get; set; }

        protected IEnumerable<AppointmentViewModel>? appointments;
        private ErrorViewModel? errorModel;

        private bool bookModalVisible = false;
        private bool patientNotFound = false;
        private string searchTerm = string.Empty;
        private DateTime appointmentDate = DateTime.Now;
        private Patient? patient;
        private PatientViewModel? existingPatient;
        private PatientViewModel newPatient = new()
        {
            Title = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            IDNumber = string.Empty,
            ContactDetails = new ContactViewModel
            {
                ContactNumber = string.Empty,
                Email = string.Empty,
                Fax = string.Empty
            },
            AddressDetails = new AddressViewModel
            {
                Street = string.Empty,
                Suburb = string.Empty,
                City = string.Empty,
                PostalCode = string.Empty,
                Country = string.Empty
            }
        };
        private AppointmentViewModel newAppointment = new()
        {
            Date = DateTime.Now,
            Description = string.Empty,
            Status = AppointmentStatus.Scheduled,
            HospitalViewModel = new HospitalViewModel
            {
                Id = 0,
                HospitalName = string.Empty
            },
            DoctorViewModel = new DoctorViewModel
            {
                Id = "0",
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Specialization = string.Empty
            },
            PatientViewModel = new PatientViewModel
            {
                Id = "0",
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty
            }
        };

        private bool rescheduleModalVisible = false;
        private bool reassignModalVisible = false;
        private AppointmentViewModel? selectedAppointment;
        private DateTime rescheduleDate = DateTime.Now;
        private string? selectedDoctorId;
        private string? patientId;

        private IEnumerable<DoctorViewModel> allDoctors = [];

        private List<string> titles = ["Dr", "Mr", "Mrs", "Miss", "Ms", "Prof"];

        private bool _loaded = false;

        protected override async Task OnInitializedAsync()
        {
            if (!_loaded)
            {
                await LoadAppointments();
                allDoctors = await LoadDoctorsAsync(); 
                _loaded = true;
            }
        }

        private async Task LoadAppointments()
        {
            if (Appointment != null)
                appointments = await Appointment.GetAllAppointmentsAsync();
        }

        private async Task<IEnumerable<DoctorViewModel>> LoadDoctorsAsync() => 
            await Doctor!.GetAllDoctorsAsync();

        private void ShowBookModal()
        {
            bookModalVisible = true;
            searchTerm = string.Empty;
            existingPatient = null;
            appointmentDate = DateTime.Now;
            patientNotFound = false;

            newPatient = new()
            {
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                IDNumber = string.Empty,
                ContactDetails = new ContactViewModel
                {
                    ContactNumber = string.Empty,
                    Email = string.Empty,
                    Fax = string.Empty
                },
                AddressDetails = new AddressViewModel
                {
                    Street = string.Empty,
                    Suburb = string.Empty,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Country = string.Empty
                }
            };
        }

        private void CloseBookModal()
        {
            bookModalVisible = false;
        }

        private async Task SearchPatient()
        {
            existingPatient = await Patient!.GetPatientByIdNumberOrContactAsync(searchTerm);
            patientNotFound = existingPatient == null;
        }

        private async Task ConfirmBooking()
        {
            if (patientNotFound && !string.IsNullOrEmpty(selectedDoctorId))
            {
                newPatient.PrimaryDoctorId = selectedDoctorId;
                patient = await Patient!.AddPatientAsync(newPatient);

                patientId = patient.Id;
            }
            else
                patientId = existingPatient?.Id;

            newAppointment.DoctorId = selectedDoctorId;
            newAppointment.PatientId = patientId;

            errorModel = await Appointment!.BookAppointmentAsync(newAppointment);

            await LoadAppointments();
            CloseBookModal();
        }

        private void ShowRescheduleModal(AppointmentViewModel appointmentVM)
        {
            selectedAppointment = appointmentVM;
            rescheduleModalVisible = true;
        }

        private async Task ConfirmReschedule()
        {
            if (selectedAppointment != null)
            {
                var appointment = await Appointment!.GetAppointmentByIdAsync(selectedAppointment.Id) 
                    ?? throw new InvalidOperationException("Appointment not found.");

                appointment.Date = selectedAppointment.Date;
                errorModel = await Appointment!.RescheduleAppointmentAsync(appointment);

                rescheduleModalVisible = false;
            }
        }

        private void CloseRescheduleModal() => rescheduleModalVisible = false;

        private void ShowReassignModal(AppointmentViewModel appointmentVM)
        {
            selectedAppointment = appointmentVM;
            selectedDoctorId = null;
            reassignModalVisible = true;
        }

        private async Task ConfirmReassign()
        {
            if (selectedAppointment != null && !string.IsNullOrEmpty(selectedDoctorId))
            {
                var doctor = await Doctor!.GetDoctorByIdAsync(selectedDoctorId) 
                    ?? throw new InvalidOperationException("Doctor not found.");

                var appointment = await Appointment!.GetAppointmentByIdAsync(selectedAppointment.Id)
                    ?? throw new InvalidOperationException("Appointment not found.");

                await Appointment!.ReAssignAppointmentAsync(appointment, doctor);
                reassignModalVisible = false;
            }
        }

        private void CloseReassignModal() => reassignModalVisible = false;

        private async Task CancelAppointment(int id)
        {
            Appointment appointment = await Appointment!.GetAppointmentByIdAsync(id) 
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment == null)
                return;

            errorModel = await Appointment!.CancelAppointmentAsync(appointment);

            await LoadAppointments();
        }
    }
}