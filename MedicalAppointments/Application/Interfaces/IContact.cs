using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IContact
    {
        Task<Contact> AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}
