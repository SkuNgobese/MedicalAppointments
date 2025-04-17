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
        public IAppointment? _appointment { get; set; }

        [Inject]
        public IPatient? _patient { get; set; }

        [Inject]
        public IDoctor? _doctor { get; set; }

        protected IEnumerable<AppointmentViewModel>? appointments;
        
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

        private ErrorViewModel? errorModel;

        protected override async Task OnInitializedAsync()
        {
            if (!_loaded)
            {
                await LoadAppointments();
                await LoadDoctorsAsync(); 
                _loaded = true;
            }
        }

        private async Task LoadAppointments()
        {
            if (_appointment != null)
                appointments = await _appointment.GetAllAppointmentsAsync();

            errorModel = _appointment!.Error;
        }

        private async Task LoadDoctorsAsync()
        {
            if (_doctor != null)
                allDoctors = await _doctor.GetAllDoctorsAsync();

            errorModel = _doctor!.Error;
        }

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
            existingPatient = await _patient!.GetPatientByIdNumberOrContactAsync(searchTerm);
            patientNotFound = existingPatient == null;

            errorModel = _patient!.Error;
        }

        private async Task ConfirmBooking()
        {
            if (patientNotFound && !string.IsNullOrEmpty(selectedDoctorId))
            {
                newPatient.PrimaryDoctorId = selectedDoctorId;
                patient = await _patient!.AddPatientAsync(newPatient);

                errorModel = _patient!.Error;
                
                if (patient == null)
                {
                    CloseBookModal();
                    return;
                }
                
                patientId = patient.Id;
            }
            else
                patientId = existingPatient?.Id;

            if (patientId == null)
                return;

            newAppointment.DoctorId = selectedDoctorId;
            newAppointment.PatientId = patientId;

            errorModel = await _appointment!.BookAppointmentAsync(newAppointment);
            
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
                var appointment = await _appointment!.GetAppointmentByIdAsync(selectedAppointment.Id);
                errorModel = _appointment.Error;

                if (appointment == null)
                {
                    rescheduleModalVisible = false;
                    return;
                }

                appointment!.Date = selectedAppointment.Date;
                errorModel = await _appointment!.RescheduleAppointmentAsync(appointment);
                errorModel = _appointment.Error;
                
                rescheduleModalVisible = false;
            }
        }

        private void CloseRescheduleModal() => 
            rescheduleModalVisible = false;

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
                var doctor = await _doctor!.GetDoctorByIdAsync(selectedDoctorId);
                errorModel = _doctor.Error;
                if (doctor == null)
                {
                    reassignModalVisible = false;
                    return;
                }

                var appointment = await _appointment!.GetAppointmentByIdAsync(selectedAppointment.Id);
                errorModel = _appointment.Error;
                if (appointment == null)
                {
                    reassignModalVisible = false;
                    return;
                }

                errorModel = await _appointment!.ReAssignAppointmentAsync(appointment!, doctor!);
                reassignModalVisible = false;
            }
        }

        private void CloseReassignModal() => reassignModalVisible = false;

        private async Task CancelAppointment(int id)
        {
            Appointment? appointment = await _appointment!.GetAppointmentByIdAsync(id);
            errorModel = _appointment.Error;

            if (appointment == null)
                return;

            errorModel = await _appointment!.CancelAppointmentAsync(appointment!);
            
            await LoadAppointments();
        }
    }
}