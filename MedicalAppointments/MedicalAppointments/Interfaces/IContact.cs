using MedicalAppointments.Models;

namespace MedicalAppointments.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}
