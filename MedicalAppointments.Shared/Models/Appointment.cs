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

        public Hospital? Hospital { get; set; }
        public int? HospitalId { get; set; }

        public Doctor? Doctor { get; set; }
        public string? DoctorId { get; set; }

        public Patient? Patient { get; set; }
        public string? PatientId { get; set; }
    }
}