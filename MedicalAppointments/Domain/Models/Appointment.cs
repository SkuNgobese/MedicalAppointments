namespace MedicalAppointments.Domain.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsCancelled { get; set; }

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }

        public void Reschedule(DateTime newDate)
        {
            if (newDate < DateTime.Now) throw new ArgumentException("New appointment date cannot be in the past.");
            Date = newDate;
        }
    }
}
