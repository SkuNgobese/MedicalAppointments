using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class HospitalService : IHospital
    {
        public HospitalService()
        {
        }

        public Task AddAsync(Hospital appointment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Hospital>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Hospital?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Hospital appointment)
        {
            throw new NotImplementedException();
        }
    }
}
