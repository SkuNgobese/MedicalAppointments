using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Interfaces
{
    public interface IAppointment
    {
        ErrorViewModel? Error { get; set; }
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<AppointmentViewModel?> GetAppointmentByIdAsync(int id);
        Task<ErrorViewModel> BookAppointmentAsync(AppointmentViewModel model);
        Task<ErrorViewModel> RescheduleAppointmentAsync(int appointmentId, DateTime newDate);
        Task<ErrorViewModel> ReAssignAppointmentAsync(AppointmentViewModel model, Doctor doctor);
        Task<ErrorViewModel> CancelAppointmentAsync(AppointmentViewModel model);
    }
}