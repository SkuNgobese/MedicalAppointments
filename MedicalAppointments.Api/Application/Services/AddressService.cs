using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class AddressService : IAddress
    {
        private readonly IRepository<Address> _repository;

        public AddressService(IRepository<Address> repository) => 
            _repository = repository;

        public async Task<Address> AddAddress(Address address) =>
            await _repository.AddAsync(address);

        public async Task UpdateAddress(Address address) =>
            await _repository.UpdateAsync(address);
    }
}