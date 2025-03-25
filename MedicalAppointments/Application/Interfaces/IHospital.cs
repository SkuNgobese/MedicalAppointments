using MedicalAppointments.Domain.Models;
using System.Linq.Expressions;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IHospital
    {
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync();

        Task<Hospital> AddHospitalAsync(Hospital hospital);

        Task<Hospital?> GetHospitalByIdAsync(int id);

        Task UpdateHospitalAsync(Hospital hospital);

        Task RemoveHospitalAsync(Hospital hospital);
    }
}
