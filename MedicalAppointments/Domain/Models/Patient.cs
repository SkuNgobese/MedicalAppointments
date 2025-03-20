namespace MedicalAppointments.Domain.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public User? User { get; set; }
        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
        public Hospital? Hospital { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}
