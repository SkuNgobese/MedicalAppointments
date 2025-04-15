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

        private bool bookModalVisible = false;
        private bool patientNotFound = false;
        private string searchTerm = string.Empty;
        private DateTime appointmentDate = DateTime.Now;
        private Patient? existingPatient;
        private PatientViewModel newPatient = new()
        {
            Title = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            IDNumber = string.Empty
        };
        private AppointmentViewModel newAppointment = new()
        {
            Date = DateTime.Now,
            Description = string.Empty,
            Status = AppointmentStatus.NoShow,
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

        private IEnumerable<DoctorViewModel> allDoctors = [];

        private List<string> titles = new() { "Dr", "Mr", "Mrs", "Miss", "Ms", "Prof" };

        protected override async Task OnInitializedAsync()
        {
            await LoadAppointments();

            allDoctors = await LoadDoctorsAsync();
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
            var patient = existingPatient;

            if (patientNotFound)
            {
                patient = new Patient
                {
                    Title = newPatient.Title,
                    FirstName = newPatient.FirstName,
                    LastName = newPatient.LastName,
                    IDNumber = newPatient.IDNumber,
                    Contact = new Contact
                    {
                        Email = newPatient.ContactDetails!.Email,
                        ContactNumber = newPatient.ContactDetails.ContactNumber,
                        Fax = newPatient.ContactDetails.Fax
                    },
                    Address = new Address
                    {
                        Street = newPatient.AddressDetails!.Street,
                        Suburb = newPatient.AddressDetails.Suburb,
                        City = newPatient.AddressDetails!.City,
                        PostalCode = newPatient.AddressDetails!.PostalCode,
                        Country = newPatient.AddressDetails.Country
                    },
                    PrimaryDoctor = new Doctor
                    {
                        Id = newAppointment.DoctorViewModel.Id!,
                        Title = newAppointment.DoctorViewModel.Title,
                        FirstName = newAppointment.DoctorViewModel.FirstName,
                        LastName = newAppointment.DoctorViewModel.LastName,
                        Specialization = newAppointment.DoctorViewModel.Specialization
                    },
                };

                patient = await Patient!.AddPatientAsync(patient);
            }

            var appointment = new Appointment
            {
                Date = newAppointment.Date,
                Description = newAppointment.Description,
                Doctor = new Doctor
                {
                    Id = newAppointment.DoctorViewModel.Id!,
                    Title = newAppointment.DoctorViewModel.Title,
                    FirstName = newAppointment.DoctorViewModel.FirstName,
                    LastName = newAppointment.DoctorViewModel.LastName,
                    Specialization = newAppointment.DoctorViewModel.Specialization
                },
                Patient = new Patient
                {
                    Id = patient!.Id,
                    Title = patient.Title!,
                    FirstName = patient.FirstName!,
                    LastName = patient.LastName!
                },
                Hospital = null!
            };

            await Appointment!.BookAppointmentAsync(appointment);

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
                await Appointment!.RescheduleAppointmentAsync(appointment);

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

            await Appointment!.CancelAppointmentAsync(appointment);

            appointments = (await Appointment!.GetAllAppointmentsAsync())?.ToList();
        }
    }
}