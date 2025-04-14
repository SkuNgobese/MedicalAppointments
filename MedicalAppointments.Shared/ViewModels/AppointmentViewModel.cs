using MedicalAppointments.Shared.Enums;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class AppointmentViewModel
    {
        public int? Id { get; set; }

        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public required DateTime Date { get; set; }
        public required string Description { get; set; }

        public required AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public required HospitalViewModel HospitalViewModel { get; set; }
        public required DoctorViewModel DoctorViewModel { get; set; }
        public required PatientViewModel PatientViewModel { get; set; }
    }
}