using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Infrastructure.Services;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IRepository<Doctor> _repository;

        private readonly ICurrentUserHelper _helper;

        public DoctorService(IRepository<Doctor> repository, ICurrentUserHelper helper)
        {
            _repository = repository;
            _helper = helper;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            var doctors = await _repository.GetAllAsync(
                                d => d.Address!,
                                d => d.Contact!,
                                d => d.Hospital!
                            );

            if (!doctors.Any())
                return [];

            return doctors;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital)
        {
            ArgumentNullException.ThrowIfNull(hospital);

            var doctors = await _repository.GetAllAsync(
                d => d.Hospital!.Id == hospital.Id,
                d => d.Address!,
                d => d.Contact!,
                d => d.Hospital!
            );

            return doctors ?? [];
        }

        public async Task<Doctor?> GetDoctorForPatientAsync(Patient patient) =>
            await _repository.GetByConditionAsync(
                d => d.Patients.Any(p => p == patient));

        public async Task<Doctor> AddDoctorAsync(Doctor doctor) =>
            await _repository.AddAsync(doctor);

        public async Task<Doctor?> GetDoctorByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital) => 
            await _repository.GetByConditionAsync(
                d => d.Id == id && d.Hospital == hospital
            );

        public async Task UpdateDoctorAsync(Doctor doctor) =>
            await _repository.UpdateAsync(doctor);

        public async Task DeleteDoctorAsync(Doctor doctor) =>
            await _repository.DeleteAsync(doctor, d => d.Contact!, d => d.Address!);

        public async Task DeleteDoctorsAsync(Hospital hospital)
        {
            var doctors = await _repository.GetAllAsync(d => d.Hospital == hospital);

            if (doctors == null)
                return;

            foreach (var doctor in doctors)
                await DeleteDoctorAsync(doctor);
        }

        public async Task<Doctor?> GetDoctorAsync(string email) => 
            await _repository.GetByConditionAsync(d => d.Email == email, d => d.Hospital!) ?? null;
        
        public async Task<bool> ExistsAsync(string email) => 
            await _repository.Exists(d => d.Email == email);

        public async Task<IEnumerable<Doctor?>> GetCurrentUserHospitalDoctorsAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            IEnumerable<Doctor> doctors = [];

            switch (role)
            {
                case "Patient":
                    var patient = user as Patient;
                    var doctor = await GetDoctorForPatientAsync(patient!);
                    if (doctor != null)
                        doctors = doctors.Append(doctor);
                    break;
                case "Doctor":
                    doctor = user as Doctor;
                    if (doctor != null)
                        doctors = doctors.Append(doctor);
                    break;
                case "Admin":
                    var admin = user as Admin;
                    doctors = await GetAllDoctorsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    doctors = await GetAllDoctorsAsync();
                    break;
                default:
                    return doctors;
            }

            return doctors;
        }
    }
}