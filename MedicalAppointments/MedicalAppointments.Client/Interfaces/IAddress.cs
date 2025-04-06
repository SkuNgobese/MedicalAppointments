using MedicalAppointments.Client.Models;

namespace MedicalAppointments.Client.Interfaces
{
    public interface IAddress
    {
        Task<Address> AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}
