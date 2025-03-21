using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
