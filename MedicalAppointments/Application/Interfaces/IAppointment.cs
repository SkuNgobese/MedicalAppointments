using MedicalAppointments.Application.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task BookAppointmentAsync(Appointment appointment);
        Task ReAssignAppointmentAsync(Doctor doctor, Appointment appointment);
        Task CancelAppointmentAsync(Appointment appointment);
    }
}
