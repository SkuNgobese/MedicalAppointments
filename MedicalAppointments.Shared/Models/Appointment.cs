using MedicalAppointments.Shared.Enums;
using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Appointment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public required Hospital Hospital { get; set; }
        public required Doctor Doctor { get; set; }
        public required Patient Patient { get; set; }
    }
}