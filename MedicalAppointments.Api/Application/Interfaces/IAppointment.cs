using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IAppointment
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Doctor doctor);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Patient patient);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task BookAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task RemoveAppointmentsAsync(Hospital hospital);
        Task<IEnumerable<Appointment?>> GetCurrentUserHospitalAppointmentsAsync();
    }
}