using MedicalAppointments.Shared.Enums;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class AppointmentViewModel
    {
        public int Id { get; set; }

        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public required DateTime Date { get; set; }
        public required string Description { get; set; }

        public AppointmentStatus? Status { get; set; } = AppointmentStatus.Scheduled;

        public HospitalViewModel? HospitalViewModel { get; set; }
        public int? HospitalId { get; set; }
        public DoctorViewModel? DoctorViewModel { get; set; }
        public string? DoctorId { get; set; }
        public PatientViewModel? PatientViewModel { get; set; }
        public string? PatientId { get; set; }
    }
}