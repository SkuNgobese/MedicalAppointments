using MedicalAppointments.Domain.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IAddress
    {
        Task AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}
