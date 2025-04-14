using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}