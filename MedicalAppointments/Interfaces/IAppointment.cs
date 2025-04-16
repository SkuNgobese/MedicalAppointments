using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<AppointmentViewModel> BookAppointmentAsync(AppointmentViewModel appointment);
        Task RescheduleAppointmentAsync(Appointment appointment);
        Task ReAssignAppointmentAsync(Appointment appointment, Doctor doctor);
        Task CancelAppointmentAsync(Appointment appointment);
    }
}