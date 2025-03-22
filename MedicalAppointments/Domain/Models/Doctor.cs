using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Domain.Models
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

        public List<Appointment>? Appointments { get; set; }
    }
}
