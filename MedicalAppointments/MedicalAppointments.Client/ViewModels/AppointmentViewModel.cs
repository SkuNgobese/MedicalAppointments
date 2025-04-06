namespace MedicalAppointments.Client.ViewModels
{
    public partial class AppointmentViewModel
    {
        [FutureDate(ErrorMessage = "Appointment date must be in the future.")]
        public required DateTime Date { get; set; }
        public required string Description { get; set; }

        public required string DoctorId { get; set; }

        public string? PatientId { get; set; }
        public required PatientViewModel PatientViewModel { get; set; }
    }
}