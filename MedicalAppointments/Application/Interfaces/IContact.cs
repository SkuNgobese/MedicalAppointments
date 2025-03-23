using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IContact
    {
        Task AddContact(Contact contact);
        
        Task UpdateContact(Contact contact);
    }
}
