using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Application.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() =>
            await _repository.GetAllAsync();

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task BookAppointmentAsync(Appointment appointment) =>
            await _repository.AddAsync(appointment);
    }
}
