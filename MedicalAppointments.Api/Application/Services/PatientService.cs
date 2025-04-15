using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class PatientService : IPatient
    {
        private readonly IRepository<Patient> _repository;

        private readonly ICurrentUserHelper _helper;

        public PatientService(IRepository<Patient> repository, ICurrentUserHelper currentUserHelper)
        {
            _repository = repository;
            _helper = currentUserHelper;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            var patients = await _repository.GetAllAsync(
                    p => p.Address!,
                    p => p.Contact!,
                    p => p.Hospital!,
                    p => p.PrimaryDoctor!,
                    p => p.Appointments
                );

            return patients ?? [];
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital)
        {
            ArgumentNullException.ThrowIfNull(hospital);

            var patients = await _repository.GetAllAsync(
                p => p.Hospital == hospital,
                p => p.Address!,
                p => p.Contact!,
                p => p.PrimaryDoctor!,
                p => p.Appointments
            );

            return patients ?? [];
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(Doctor doctor)
        {
            ArgumentNullException.ThrowIfNull(doctor);

            var patients = await _repository.GetAllAsync(
                p => p.PrimaryDoctor == doctor,
                p => p.Address!,
                p => p.Contact!,
                p => p.Appointments
            );

            return patients ?? [];
        }

        public async Task<Patient> AddPatientAsync(Patient patient) =>
            await _repository.AddAsync(patient);

        public async Task<Patient?> GetByIdNumberOrContactAsync(Hospital hospital, string search) =>
            await _repository.GetByConditionAsync(
                                p => p.Hospital == hospital &&
                                p.IDNumber == search || 
                                p.Contact!.ContactNumber == search ||
                                p.Contact.Email == search, 
                                p => p.Contact!,
                                p => p.Address!,
                                p => p.Appointments);

        public async Task<Patient?> GetPatientByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task UpdatePatientAsync(Patient patient) =>
            await _repository.UpdateAsync(patient);

        public async Task RemovePatientAsync(Patient patient) =>
            await _repository.DeleteAsync(patient);

        public async Task RemovePatientsAsync(Doctor doctor)
        {
            var patients = await _repository.GetAllAsync(d => d.PrimaryDoctor == doctor);

            if (patients == null)
                return;

            foreach (var patient in patients)
            {
                patient.PrimaryDoctor = null;
                await _repository.UpdateAsync(patient);
            }
        }

        public async Task RemovePatientsAsync(Hospital hospital)
        {
            var patients = await _repository.GetAllAsync(d => d.Hospital == hospital);

            if (patients == null)
                return;

            foreach (var patient in patients)
                await _repository.DeleteAsync(patient);
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _repository.Exists(d => d.Email == email);

        public async Task<Patient?> GetPatientAsync(string email) =>
            await _repository.GetByConditionAsync(p => p.Email == email);

        public async Task<IEnumerable<Patient?>> GetCurrentUserHospitalPatientsAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            IEnumerable<Patient> patients = null!;

            switch (role)
            {
                case "Patient":
                    var patient = user as Patient;
                    patients = patients!.Append(patient!);
                    break;
                case "Doctor":
                    var doctor = user as Doctor;
                    patients = await GetAllPatientsAsync(doctor!);
                    break;
                case "Admin":
                    var admin = user as Admin;
                    patients = await GetAllPatientsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    patients = await GetAllPatientsAsync();
                    break;
                default:
                    return patients;
            }

            return patients;
        }
    }
}