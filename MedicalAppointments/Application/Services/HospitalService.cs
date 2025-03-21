using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;
using System.Numerics;

namespace MedicalAppointments.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _repository;

        public HospitalService(IRepository<Hospital> repository) => _repository = repository;

        public async Task AddHospitalAsync(Hospital hospital) =>
            await _repository.AddAsync(hospital);

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync() =>
            await _repository.GetAllAsync();

        public async Task<Hospital?> GetHospitalByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task RemoveHospitalAsync(int id) =>
            await _repository.DeleteAsync(id);

        public async Task UpdateHospitalAsync(Hospital hospital) =>
            await _repository.UpdateAsync(hospital);
    }
}