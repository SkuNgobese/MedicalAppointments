using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<Appointment>? Appointments { get; set; }
    }
}
