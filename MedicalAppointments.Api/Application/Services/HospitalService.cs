using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Application.Interfaces;

namespace MedicalAppointments.Api.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _hospitalRepository;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IAppointment _appointment;
        private readonly IAdmin _sysAdmin;

        private readonly ICurrentUserHelper _helper;

        public HospitalService(IRepository<Hospital> hospitalRepository,
                               IDoctor doctor,
                               IPatient patient,
                               IAppointment appointment,
                               IAdmin sysAdmin,
                               ICurrentUserHelper helper)
        {
            _hospitalRepository = hospitalRepository;
            _doctor = doctor;
            _patient = patient;
            _appointment = appointment;
            _sysAdmin = sysAdmin;
            _helper = helper;
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
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            Hospital hospital;
            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    hospital = doctor!.Hospital!;
                    break;
                case "Admin":
                    var admin = user as Admin;
                    hospital = admin!.Hospital!;
                    break;
                default:
                    return null;
            }

            return hospital;
        }
    }
}