using MedicalAppointments.Models;

namespace MedicalAppointments.Interfaces
{
    public interface IAddress
    {
        Task<Address> AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}
