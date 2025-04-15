using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Doctor : ApplicationUser
    {
        [JsonPropertyName("specialization")]
        public string? Specialization { get; set; }

        [JsonPropertyName("hiredate")]
        public DateTime? HireDate { get; set; }

        [JsonPropertyName("retiredate")]
        public DateTime? RetireDate { get; set; }

        [JsonPropertyName("isretired")]
        public bool IsRetired { get; set; }

        public ICollection<Patient> Patients { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}