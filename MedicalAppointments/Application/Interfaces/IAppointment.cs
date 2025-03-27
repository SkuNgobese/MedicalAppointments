using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task BookAppointmentAsync(Appointment appointment);
        Task ReAssignAppointmentAsync(Appointment appointment);
        Task CancelAppointmentAsync(Appointment appointment);
    }
}
