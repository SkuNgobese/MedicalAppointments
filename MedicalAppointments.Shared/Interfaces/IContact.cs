using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}