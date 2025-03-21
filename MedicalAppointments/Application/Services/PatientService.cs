using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class PatientService : IPatient
    {
        private readonly IRepository<Patient> _repository;
        public PatientService(IRepository<Patient> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync() =>
            await _repository.GetAllAsync();
        public async Task AddPatientAsync(Patient patient) =>
            await _repository.AddAsync(patient);
        public async Task<Patient?> GetPatientByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);
        public async Task UpdatePatientAsync(Patient patient) =>
            await _repository.UpdateAsync(patient);
        public async Task RemovePatientAsync(string id) =>
            await _repository.DeleteAsync(id);
    }
}
