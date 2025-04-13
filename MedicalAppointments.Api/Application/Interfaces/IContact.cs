using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}