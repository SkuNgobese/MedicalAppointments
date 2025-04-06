using MedicalAppointments.Models;

namespace MedicalAppointments.Interfaces
{
    public interface IAppointment
    {
        Task<List<Appointment>> GetAppointmentsAsync();
    }
}
