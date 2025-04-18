using BlazorBootstrap;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

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

        List<ToastMessage> messages = [];

        protected IEnumerable<AppointmentViewModel>? appointments;

        private Modal modalForm = default!;
        private Modal modalReschedule = default!;
        private Modal modalReassign = default!;
        private Modal modalCancel = default!; 

        private bool patientNotFound = false;
        private string searchTerm = string.Empty;
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

        private AppointmentViewModel? selectedAppointment;
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
        }

        private async Task LoadDoctorsAsync()
        {
            errorModel = null;

            if (_doctor != null)
                allDoctors = await _doctor.GetAllDoctorsAsync();

            errorModel = _doctor!.Error;
        }

        private async Task ShowBookModal()
        {
            ResetForm();
            await modalForm.ShowAsync();
        }

        private async Task CloseBookModal() => 
            await CloseModal(modalForm);

        private async Task SearchPatient()
        {
            errorModel = null;

            if (string.IsNullOrEmpty(searchTerm))
                return;

            existingPatient = await _patient!.GetPatientByIdNumberOrContactAsync(searchTerm);
            patientNotFound = existingPatient == null;

            errorModel = _patient!.Error;
        }

        private async Task ConfirmBooking()
        {
            errorModel = null;

            if (patientNotFound)
            {
                newPatient.PrimaryDoctorId = selectedDoctorId;
                newPatient = await _patient!.AddPatientAsync(newPatient);

                errorModel = _patient!.Error;

                if (errorModel!.Errors != null && errorModel.Errors.Count > 0)
                    return;

                patientId = newPatient.Id;
            }
            else if(existingPatient != null)
                patientId = existingPatient?.Id;

            if (string.IsNullOrEmpty(patientId))
            {
                errorModel = new ErrorViewModel
                {
                    Message = "Please select a patient",
                    Errors = ["Patient not selected"]
                };

                return;
            }

            if (string.IsNullOrEmpty(selectedDoctorId))
            {
                errorModel = new ErrorViewModel
                {
                    Message = "Please select a doctor",
                    Errors = ["Doctor not selected"]
                };

                return;
            }

            newAppointment.DoctorId = selectedDoctorId;
            newAppointment.PatientId = patientId;

            errorModel = await _appointment!.BookAppointmentAsync(newAppointment);

            if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                return;

            SuccessMessage("Appointment booked successfully!");

            await LoadAppointments();
            await CloseBookModal();
        }

        private void ShowRescheduleModal(AppointmentViewModel appointmentVM)
        {
            errorModel = null;
            selectedAppointment = appointmentVM;
            modalReschedule.ShowAsync();
        }

        private async Task ConfirmReschedule()
        {
            errorModel = null;

            if (selectedAppointment != null)
            {
                if (selectedAppointment == null)
                    return;

                errorModel = await _appointment!.RescheduleAppointmentAsync(selectedAppointment.Id, selectedAppointment.Date);
                errorModel = _appointment.Error;

                if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                    return;

                SuccessMessage("Appointment rescheduled successfully!");

                await CloseRescheduleModal();
                await LoadAppointments();
            }
        }

        private void ShowReassignModal(AppointmentViewModel appointmentVM)
        {
            errorModel = null;
            selectedAppointment = appointmentVM;
            selectedDoctorId = string.Empty;
            modalReassign.ShowAsync();
        }

        private async Task ConfirmReassign()
        {
            errorModel = null;

            if (selectedAppointment != null && !string.IsNullOrEmpty(selectedDoctorId))
            {
                var doctor = await _doctor!.GetDoctorByIdAsync(selectedDoctorId);
                errorModel = _doctor.Error;

                if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                    return;

                if (doctor == null)
                    return;

                selectedAppointment = await _appointment!.GetAppointmentByIdAsync(selectedAppointment.Id);
                errorModel = _appointment.Error;

                if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                    return;

                if (selectedAppointment == null)
                    return;

                errorModel = await _appointment!.ReAssignAppointmentAsync(selectedAppointment!, doctor!);

                if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                    return;

                SuccessMessage("Appointment reassigned successfully!");

                await CloseReassignModal();
                await LoadAppointments();
            }
        }

        private void ShowCancelModal(AppointmentViewModel appointmentVM)
        {
            errorModel = null;

            selectedAppointment = appointmentVM;
            modalCancel.ShowAsync();
        }

        private async Task ConfirmCancel()
        {
            errorModel = null;

            if (selectedAppointment == null)
                return;

            selectedAppointment = await _appointment!.GetAppointmentByIdAsync(selectedAppointment.Id);
            errorModel = _appointment.Error;

            if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                return;

            if (selectedAppointment == null)
                return;

            errorModel = await _appointment!.CancelAppointmentAsync(selectedAppointment!);

            if (errorModel?.Errors != null && errorModel.Errors.Count > 0)
                return;

            SuccessMessage("Appointment cancelled successfully!");

            await CloseCancelModal();
            await LoadAppointments();
        }

        private void SuccessMessage(string message)
        {
            messages.Add(new ToastMessage
            {
                Type = ToastType.Success,
                Title = "Notification",
                Message = message
            });
        }

        private async Task CloseRescheduleModal() =>
            await CloseModal(modalReschedule);

        private async Task CloseReassignModal() =>
            await CloseModal(modalReassign);

        private async Task CloseCancelModal() => 
            await CloseModal(modalCancel);

        private static async Task CloseModal(Modal modal) => 
            await modal.HideAsync();

        private void ResetForm()
        {
            errorModel = null;
            searchTerm = string.Empty;
            existingPatient = null;
            patientNotFound = false;

            newAppointment = new()
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
    }
}