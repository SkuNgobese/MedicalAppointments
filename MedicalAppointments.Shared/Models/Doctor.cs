using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Doctor : ApplicationUser
    {
        [JsonPropertyName("idnumber")]
        public string? IDNumber { get; set; }

        [JsonPropertyName("specialization")]
        public string? Specialization { get; set; }

        [JsonPropertyName("hiredate")]
        public DateTime? HireDate { get; set; }

        [JsonPropertyName("removedate")]
        public DateTime RemoveDate { get; set; }

        [JsonPropertyName("retiredate")]
        public DateTime? RetireDate { get; set; }

        [JsonPropertyName("isretired")]
        public bool IsRetired { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }

        public Hospital? Hospital { get; set; }
        public ICollection<Patient> Patients { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}