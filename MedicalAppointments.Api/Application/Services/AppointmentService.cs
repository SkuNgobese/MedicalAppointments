using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly IRepository<Appointment> _repository;

        private readonly ICurrentUserHelper _helper;

        public AppointmentService(IRepository<Appointment> repository, ICurrentUserHelper helper)
        {
            _repository = repository;
            _helper = helper;
        }

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

        public async Task<IEnumerable<Appointment?>> GetCurrentUserHospitalAppointmentsAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync();

            IEnumerable<Appointment> appointments = null!;

            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    appointments = await GetAllAppointmentsAsync(doctor!);
                    break;
                case "Patient":
                    var patient = user as Patient;
                    appointments = await GetAllAppointmentsAsync(patient!);
                    break;
                case "Admin":
                    var admin = user as Admin;
                    appointments = await GetAllAppointmentsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    appointments = await GetAllAppointmentsAsync();
                    break;
                default:
                    return appointments;
            }

            return appointments;
        }
    }
}