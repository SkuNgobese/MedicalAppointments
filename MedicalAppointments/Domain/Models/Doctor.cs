using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Domain.Models
{
    public class Doctor : User
    {
        public string? Specialization { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime RemoveDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public bool IsRetired { get; set; }

        //public User? User { get; set; }
        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
        public Hospital? Hospital { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}
