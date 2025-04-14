using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Interfaces
{
    public interface IAddress
    {
        Task<Address> AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}