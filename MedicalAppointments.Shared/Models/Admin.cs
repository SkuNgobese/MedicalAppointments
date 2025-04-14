namespace MedicalAppointments.Shared.Models
{
    public class Admin : ApplicationUser
    {
        public Hospital? Hospital { get; set; }
    }
}