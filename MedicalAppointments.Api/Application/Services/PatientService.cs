using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;

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
    }
}