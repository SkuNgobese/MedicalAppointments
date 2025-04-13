using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Interfaces
{
    public interface IAddress
    {
        Task<Address> AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}