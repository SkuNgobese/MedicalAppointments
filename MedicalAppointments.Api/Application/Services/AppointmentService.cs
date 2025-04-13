using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Interfaces;
using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly IRepository<Appointment> _repository;

        public AppointmentService(IRepository<Appointment> repository) => _repository = repository;

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() =>
            await _repository.GetAllAsync();

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital) =>
            await _repository.GetAllAsync(a => a.Hospital == hospital);

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Doctor doctor) =>
            await _repository.GetAllAsync(a => a.Doctor == doctor);

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Patient patient) =>
            await _repository.GetAllAsync(a => a.Patient == patient);

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task BookAppointmentAsync(Appointment appointment) => 
            await _repository.AddAsync(appointment);

        public async Task ReAssignAppointmentAsync(Appointment appointment) =>
            await _repository.UpdateAsync(appointment);

        public async Task CancelAppointmentAsync(Appointment appointment) =>
            await _repository.UpdateAsync(appointment);

        public async Task RemoveAppointmentsAsync(Hospital hospital)
        {
            var appointments = await _repository.GetAllAsync(a => a.Hospital == hospital);
            
            if (appointments == null)
                return;

            foreach (var appointment in appointments)
                await _repository.DeleteAsync(appointment);
        }
    }
}