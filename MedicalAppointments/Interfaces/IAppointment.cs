using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Doctor doctor);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task BookAppointmentAsync(Appointment appointment);
        Task ReAssignAppointmentAsync(Appointment appointment);
        Task CancelAppointmentAsync(Appointment appointment);
    }
}