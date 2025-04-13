using System.Text.Json.Serialization;

namespace MedicalAppointments.Api.Models
{
    public class Hospital
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
        public ICollection<Doctor> Doctors { get; set; } = [];
        public ICollection<Patient> Patients { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}