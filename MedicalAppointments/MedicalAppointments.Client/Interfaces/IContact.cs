using MedicalAppointments.Client.Models;

namespace MedicalAppointments.Client.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}
