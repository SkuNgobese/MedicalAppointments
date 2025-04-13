using MedicalAppointments.Api.Application.Helpers;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _hospitalRepository;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IAppointment _appointment;
        private readonly ISysAdmin _sysAdmin;

        private readonly CurrentUserHelper _helpers;

        public HospitalService(IRepository<Hospital> hospitalRepository,
                               IDoctor doctor,
                               IPatient patient,
                               IAppointment appointment,
                               ISysAdmin sysAdmin,
                               CurrentUserHelper helpers)
        {
            _hospitalRepository = hospitalRepository;
            _doctor = doctor;
            _patient = patient;
            _appointment = appointment;
            _sysAdmin = sysAdmin;
            _helpers = helpers;
        }

        public async Task<Hospital> AddHospitalAsync(Hospital hospital) =>
            await _hospitalRepository.AddAsync(hospital);

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync(h => h.Address!, h => h.Contact!);

            return hospitals.Select(h => new Hospital
            {
                Id = h.Id,
                Name = h.Name,
                Address = new Address
                {
                    Id = h.Address?.Id ?? 0,
                    Street = h.Address?.Street ?? string.Empty,
                    Suburb = h.Address?.Suburb ?? string.Empty,
                    City = h.Address?.City ?? string.Empty,
                    PostalCode = h.Address?.PostalCode ?? string.Empty,
                    Country = h.Address?.Country ?? string.Empty
                },
                Contact = new Contact
                {
                    Id = h.Contact?.Id ?? 0,
                    Email = h.Contact?.Email ?? string.Empty,
                    ContactNumber = h.Contact?.ContactNumber ?? string.Empty,
                    Fax = h.Contact?.Fax ?? string.Empty
                }
            });
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id) =>
            await _hospitalRepository.GetByIdAsync(id);

        public async Task RemoveHospitalAsync(Hospital hospital)
        {
            await _appointment.RemoveAppointmentsAsync(hospital);
            await _patient.RemovePatientsAsync(hospital);
            await _doctor.RemoveDoctorsAsync(hospital);
            await _sysAdmin.RemoveAdminAsync(hospital);

            await _hospitalRepository.DeleteAsync(hospital);
        }

        public async Task UpdateHospitalAsync(Hospital hospital) =>
            await _hospitalRepository.UpdateAsync(hospital);

        public async Task<Hospital?> GetCurrentUserHospitalAsync()
        {
            var user = await _helpers.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helpers.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            Hospital hospital;
            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    hospital = doctor!.Hospital!;
                    break;
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    hospital = admin!.Hospital!;
                    break;
                default:
                    return null;
            }

            return hospital;
        }
    }
}