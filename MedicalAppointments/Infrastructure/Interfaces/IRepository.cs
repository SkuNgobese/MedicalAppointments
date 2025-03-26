using System.Linq.Expressions;

namespace MedicalAppointments.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(string id);
        Task<T?> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task<T> AddAsync(T entity, params Expression<Func<T, object>>[] relatedEntities);
        Task UpdateAsync(T entity);
        Task UpdateAsync(T entity, params Expression<Func<T, object>>[] relatedEntities);
        Task DeleteAsync(T entity);
    }
}
