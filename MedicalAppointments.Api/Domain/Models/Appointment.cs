using MedicalAppointments.Api.Domain.Enums;

namespace MedicalAppointments.Api.Domain.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public required Hospital Hospital { get; set; }
        public required Doctor Doctor { get; set; }
        public required Patient Patient { get; set; }
    }
}
