namespace MedicalAppointments.Domain.Models
{
    public class Patient : User
    {
        public DateTime CreateDate { get; set; }
        public DateTime? RemoveDate { get; set; }
        public bool IsActive { get; set; }

        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
        public Hospital? Hospital { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}
