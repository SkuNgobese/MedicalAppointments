using MedicalAppointments.Client.Models;

namespace MedicalAppointments.Client.Interfaces
{
    public interface IAppointment
    {
        Task<List<Appointment>> GetAppointmentsAsync();
    }
}
