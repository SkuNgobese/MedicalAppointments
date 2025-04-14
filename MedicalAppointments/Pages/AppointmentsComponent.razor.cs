using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Pages
{
    public partial class AppointmentsComponent
    {
        [Inject]
        public IAppointment? Appointment { get; set; }

        [Inject]
        public IDoctor? Doctor { get; set; }

        protected IEnumerable<Appointment>? appointments;

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

        private void ShowRescheduleModal(Appointment appointment)
        {
            selectedAppointment = appointment;
            rescheduleDate = appointment.Date;
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

        private void ShowReassignModal(Appointment appointment)
        {
            selectedAppointment = appointment;
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