using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Domain.Models;
using MedicalAppointments.Api.Infrastructure.Interfaces;

namespace MedicalAppointments.Api.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _repository;

        public HospitalService(IRepository<Hospital> repository) => _repository = repository;

        public async Task<Hospital> AddHospitalAsync(Hospital hospital) =>
            await _repository.AddAsync(hospital);

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync() =>
            await _repository.GetAllAsync();

        public async Task<Hospital?> GetHospitalByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task RemoveHospitalAsync(Hospital hospital) =>
            await _repository.DeleteAsync(hospital);

        public async Task UpdateHospitalAsync(Hospital hospital) =>
            await _repository.UpdateAsync(hospital);
    }
}