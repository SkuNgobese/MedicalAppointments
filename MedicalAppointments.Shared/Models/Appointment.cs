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
        public string Description { get; set; } = "General checkup";

        [JsonPropertyName("status")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [JsonPropertyName("createddate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("createdby")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("updateddate")]
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedby")]
        public string? UpdatedBy { get; set; }

        public Hospital? Hospital { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}