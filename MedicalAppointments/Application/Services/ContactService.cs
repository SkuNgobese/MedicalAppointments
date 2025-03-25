using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class ContactService : IContact
    {
        private readonly IRepository<Contact> _repository;

        public ContactService(IRepository<Contact> repository) => _repository = repository;

        public async Task<Contact> AddContact(Contact contact) =>
            await _repository.AddAsync(contact);

        public async Task UpdateContact(Contact contact) =>
            await _repository.UpdateAsync(contact);
    }
}
