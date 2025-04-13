using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Interfaces;
using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class PatientService : IPatient
    {
        private readonly IRepository<Patient> _repository;

        public PatientService(IRepository<Patient> repository) => _repository = repository;

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital)
        {
            var patients = await _repository.GetAllAsync();

            return patients.Where(p => p.Hospital != null && p.Hospital.Equals(hospital));
        }
            

        public async Task AddPatientAsync(Patient patient) =>
            await _repository.AddAsync(patient);

        public async Task<Patient?> GetPatientByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task UpdatePatientAsync(Patient patient) =>
            await _repository.UpdateAsync(patient);

        public async Task RemovePatientAsync(Patient patient) =>
            await _repository.DeleteAsync(patient);

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
    }
}