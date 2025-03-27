using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Domain.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly IRepository<Appointment> _repository;

        public AppointmentService(IRepository<Appointment> repository) => _repository = repository;

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() =>
            await _repository.GetAllAsync();

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital) =>
            await _repository.GetAllAsync(a => a.Hospital == hospital);

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task BookAppointmentAsync(Appointment appointment) => 
            await _repository.AddAsync(appointment);

        public async Task ReAssignAppointmentAsync(Appointment appointment) =>
            await _repository.UpdateAsync(appointment);

        public async Task CancelAppointmentAsync(Appointment appointment) =>
            await _repository.UpdateAsync(appointment);
    }
}