namespace MedicalAppointments.Domain.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public List<Appointment>? Appointments { get; set; }
    }
}
