namespace MedicalAppointments.Api.Domain.Models
{
    public class Doctor : User
    {
        public string? IDNumber { get; set; }
        public string? Specialization { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime RemoveDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public bool IsRetired { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Patient> Patients { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
