namespace MedicalAppointments.Domain.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
        public List<Doctor>? Doctors { get; set; }
        public List<Patient>? Patients { get; set; }
        public List<Appointment>? Appointments { get; set; }
    }
}
