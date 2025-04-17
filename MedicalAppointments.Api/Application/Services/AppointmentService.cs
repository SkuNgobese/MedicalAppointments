using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
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
                await _repository.GetAllAsync(
                                        a => a.Hospital!,
                                        a => a.Hospital!.Address!,
                                        a => a.Doctor!,
                                        a => a.Doctor!.Contact!,
                                        a => a.Doctor!.Address!,
                                        a => a.Patient!,
                                        a => a.Patient!.Contact!,
                                        a => a.Patient!.Address!) ?? [];

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital) =>
                await _repository.GetAllAsync(
                                        a => a.Hospital == hospital,
                                        a => a.Doctor!,
                                        a => a.Doctor!.Contact!,
                                        a => a.Doctor!.Address!,
                                        a => a.Patient!,
                                        a => a.Patient!.Contact!,
                                        a => a.Patient!.Address!) ?? [];

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Doctor doctor) =>
                await _repository.GetAllAsync(
                                        a => a.Doctor == doctor,
                                        a => a.Patient!,
                                        a => a.Patient!.Contact!,
                                        a => a.Patient!.Address!) ?? [];

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Patient patient) =>
                await _repository.GetAllAsync(
                                        a => a.Patient == patient,
                                        a => a.Hospital!,
                                        a => a.Hospital!.Contact!,
                                        a => a.Hospital!.Address!,
                                        a => a.Doctor!,
                                        a => a.Doctor!.Contact!) ?? [];

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
                await _repository.GetByIdAsync(id,
                                        a => a.Hospital!,
                                        a => a.Hospital!.Contact!,
                                        a => a.Doctor!,
                                        a => a.Doctor!.Contact!,
                                        a => a.Patient!,
                                        a => a.Patient!.Contact!,
                                        a => a.Patient!.Address!);

        public async Task<Appointment> AddAppointmentAsync(Appointment appointment) => 
                await _repository.AddAsync(appointment);

        public async Task UpdateAppointmentAsync(Appointment appointment) =>
                await _repository.UpdateAsync(appointment);

        public async Task DeleteAppointmentAsync(Appointment appointment) =>
                await _repository.DeleteAsync(appointment);

        public async Task DeleteAppointmentsAsync(Hospital hospital)
        {
            var appointments = await _repository.GetAllAsync(a => a.Hospital == hospital);
            
            if (appointments == null)
                return;

            foreach (var appointment in appointments)
                await DeleteAppointmentAsync(appointment);
        }

        public async Task DeleteAppointmentsAsync(Doctor doctor)
        {
            var appointments = await _repository.GetAllAsync(a => a.Doctor == doctor);

            if (appointments == null)
                return;

            foreach (var appointment in appointments)
                await DeleteAppointmentAsync(appointment);
        }

        public async Task<IEnumerable<Appointment?>> GetCurrentUserHospitalAppointmentsAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync();

            IEnumerable<Appointment> appointments = null!;

            switch (role)
            {
                case "Patient":
                    var patient = user as Patient;
                    appointments = await GetAllAppointmentsAsync(patient!);
                    break;
                case "Doctor":
                    var doctor = user as Doctor;
                    appointments = await GetAllAppointmentsAsync(doctor!);
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