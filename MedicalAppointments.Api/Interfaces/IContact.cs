using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}