using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Application.Models;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly IRepository<Appointment> _repository;
        private IAppointmentValidation _appointmentValidation;

        public AppointmentService(IRepository<Appointment> repository, IAppointmentValidation appointmentValidation)
        {
            _repository = repository;
            _appointmentValidation = appointmentValidation;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() =>
            await _repository.GetAllAsync();

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task BookAppointmentAsync(Appointment appointment) =>
            await _repository.AddAsync(appointment);

        public async Task ReAssignAppointmentAsync(Doctor doctor, Appointment appointment)
        {
            if(_appointmentValidation.CanReassign(doctor, appointment))
            {
                appointment.Doctor = doctor;

                await _repository.UpdateAsync(appointment);
            }
        }

        public async Task CancelAppointmentAsync(Appointment appointment)
        {
            if(_appointmentValidation.CanCancel(appointment))
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _repository.UpdateAsync(appointment);
            }
        }
    }
}