namespace MedicalAppointments.Api.Domain.Enums
{
    public enum AppointmentStatus
    {
        Scheduled,   // Default state when an appointment is created
        Confirmed,   // Patient has confirmed attendance
        Completed,   // Appointment took place successfully
        Cancelled,    // Canceled by the patient or doctor
        NoShow       // Patient did not show up
    }
}
