using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _repository;

        public HospitalService(IRepository<Hospital> repository) => _repository = repository;

        public async Task<Hospital> AddHospitalAsync(Hospital hospital) =>
            await _repository.AddAsync(hospital);

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            var hospitals = await _repository.GetAllAsync(h => h.Address!, h => h.Contact!);

            return hospitals.Select(h => new Hospital
            {
                Name = h.Name,
                Address = new Address
                {
                    Street = h.Address?.Street ?? string.Empty,
                    Suburb = h.Address?.Suburb ?? string.Empty,
                    City = h.Address?.City ?? string.Empty,
                    PostalCode = h.Address?.PostalCode ?? string.Empty,
                    Country = h.Address?.Country ?? string.Empty
                },
                Contact = new Contact
                {
                    Email = h.Contact?.Email ?? string.Empty,
                    ContactNumber = h.Contact?.ContactNumber ?? string.Empty,
                    Fax = h.Contact?.Fax ?? string.Empty
                }
            });
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task RemoveHospitalAsync(Hospital hospital) =>
            await _repository.DeleteAsync(hospital);

        public async Task UpdateHospitalAsync(Hospital hospital) =>
            await _repository.UpdateAsync(hospital);
    }
}