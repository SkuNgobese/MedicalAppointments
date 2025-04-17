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
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentsAsync(Hospital hospital);
        Task DeleteAppointmentsAsync(Doctor doctor);
        Task<IEnumerable<Appointment?>> GetCurrentUserHospitalAppointmentsAsync();
    }
}