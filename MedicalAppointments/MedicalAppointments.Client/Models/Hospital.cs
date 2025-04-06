namespace MedicalAppointments.Client.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public Address? Address { get; set; }
        public Contact? Contact { get; set; }
        public ICollection<Doctor> Doctors { get; set; } = [];
        public ICollection<Patient> Patients { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
