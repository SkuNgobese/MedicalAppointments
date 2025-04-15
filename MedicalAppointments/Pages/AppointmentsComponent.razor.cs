using MedicalAppointments.Interfaces;
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
        public IDoctor? Doctor { get; set; }

        protected IEnumerable<AppointmentViewModel>? appointments;

        private bool rescheduleModalVisible = false;
        private bool reassignModalVisible = false;
        private Appointment? selectedAppointment;
        private DateTime rescheduleDate = DateTime.Now;
        private string? selectedDoctorId;

        private IEnumerable<Doctor> allDoctors = [];

        protected override async Task OnInitializedAsync()
        {
            if (Appointment != null)
                appointments = await Appointment.GetAllAppointmentsAsync();

            allDoctors = await LoadDoctorsAsync();
        }

        private async Task<List<Doctor>> LoadDoctorsAsync() => 
            (List<Doctor>)await Doctor!.GetAllDoctorsAsync();

        private void ShowRescheduleModal(AppointmentViewModel appointmentVM)
        {
            selectedAppointment = new Appointment
            {
                Id = appointmentVM.Id,
                Date = appointmentVM.Date,
                Description = appointmentVM.Description,
                Status = appointmentVM.Status,
                Hospital = new Hospital
                { 
                    Id = (int)appointmentVM.HospitalViewModel.Id!,
                    Name = appointmentVM.HospitalViewModel.HospitalName,
                },
                Doctor = new Doctor
                {
                    Id = appointmentVM.DoctorViewModel.Id!,
                    Title = appointmentVM.DoctorViewModel.Title,
                    FirstName = appointmentVM.DoctorViewModel.FirstName,
                    LastName = appointmentVM.DoctorViewModel.LastName,
                    Specialization = appointmentVM.DoctorViewModel.Specialization
                },
                Patient = new Patient
                {
                    Id = appointmentVM.PatientViewModel.Id!,
                    Title = appointmentVM.PatientViewModel.Title,
                    FirstName = appointmentVM.PatientViewModel.FirstName,
                    LastName = appointmentVM.PatientViewModel.LastName
                }
            };
            rescheduleDate = appointmentVM.Date;
            rescheduleModalVisible = true;
        }

        private async Task ConfirmReschedule()
        {
            if (selectedAppointment != null)
            {
                selectedAppointment.Date = rescheduleDate;
                await Appointment!.ReAssignAppointmentAsync(selectedAppointment);

                rescheduleModalVisible = false;
            }
        }

        private void CloseRescheduleModal() => rescheduleModalVisible = false;

        private void ShowReassignModal(AppointmentViewModel appointmentVM)
        {
            selectedAppointment = new Appointment
            {
                Id = appointmentVM.Id,
                Date = appointmentVM.Date,
                Description = appointmentVM.Description,
                Status = appointmentVM.Status,
                Hospital = new Hospital
                { 
                    Id = (int)appointmentVM.HospitalViewModel.Id!,
                    Name = appointmentVM.HospitalViewModel.HospitalName,
                },
                Doctor = new Doctor
                {
                    Id = appointmentVM.DoctorViewModel.Id!,
                    Title = appointmentVM.DoctorViewModel.Title,
                    FirstName = appointmentVM.DoctorViewModel.FirstName,
                    LastName = appointmentVM.DoctorViewModel.LastName,
                    Specialization = appointmentVM.DoctorViewModel.Specialization
                },
                Patient = new Patient
                {
                    Id = appointmentVM.PatientViewModel.Id!,
                    Title = appointmentVM.PatientViewModel.Title,
                    FirstName = appointmentVM.PatientViewModel.FirstName,
                    LastName = appointmentVM.PatientViewModel.LastName
                }
            };
            selectedDoctorId = null;
            reassignModalVisible = true;
        }

        private async Task ConfirmReassign()
        {
            if (selectedAppointment != null && !string.IsNullOrEmpty(selectedDoctorId))
            {
                var doctor = await Doctor!.GetDoctorByIdAsync(selectedDoctorId) ?? throw new InvalidOperationException("Doctor not found.");
                selectedAppointment.Doctor = doctor;

                await Appointment!.ReAssignAppointmentAsync(selectedAppointment);
                reassignModalVisible = false;
            }
        }

        private void CloseReassignModal() => reassignModalVisible = false;

        private async Task CancelAppointment(int id)
        {
            Appointment appointment = await Appointment!.GetAppointmentByIdAsync(id) ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment == null)
                return;

            await Appointment!.CancelAppointmentAsync(appointment);

            appointments = (await Appointment!.GetAllAppointmentsAsync())?.ToList();
        }
    }
}