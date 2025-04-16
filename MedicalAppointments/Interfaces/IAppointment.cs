using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<ErrorViewModel> BookAppointmentAsync(AppointmentViewModel appointment);
        Task<ErrorViewModel> RescheduleAppointmentAsync(Appointment appointment);
        Task ReAssignAppointmentAsync(Appointment appointment, Doctor doctor);
        Task<ErrorViewModel> CancelAppointmentAsync(Appointment appointment);
    }
}