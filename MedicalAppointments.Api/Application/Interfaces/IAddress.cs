using MedicalAppointments.Api.Domain.Models;

namespace MedicalAppointments.Api.Application.Interfaces
{
    public interface IAddress
    {
        Task<Address> AddAddress(Address address);

        Task UpdateAddress(Address address);
    }
}
