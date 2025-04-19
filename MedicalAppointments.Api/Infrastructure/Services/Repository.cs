using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace MedicalAppointments.Api.Infrastructure.Services
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);
            
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T?> GetByIdAsync(
            string id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
        }

        public async Task<T?> GetByConditionAsync(
            Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> AddAsync(T entity, params Expression<Func<T, object>>[] relatedEntities)
        {
            foreach (var relatedEntity in relatedEntities)
            {
                var navigationProperty = relatedEntity.Compile().Invoke(entity);
                if (navigationProperty != null)
                    await _context.AddAsync(navigationProperty);
            }

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Attach(entity);
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity, params Expression<Func<T, object>>[] relatedEntities)
        {
            foreach (var relatedEntity in relatedEntities)
            {
                var navigationProperty = relatedEntity.Compile().Invoke(entity);
                if (navigationProperty != null)
                    _context.Attach(navigationProperty);
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteAsync(T entity, params Expression<Func<T, object>>[] relatedEntities)
        {
            if (entity == null) 
                return;

            _dbSet.Attach(entity);

            foreach (var related in relatedEntities)
            {
                var member = related.Body is MemberExpression memberExpr
                    ? memberExpr
                    : (related.Body is UnaryExpression unary && 
                    unary.Operand is MemberExpression unaryMember ? unaryMember : null);

                if (member == null)
                    continue;

                var navigationName = member.Member.Name;
                var navEntry = _context.Entry(entity).Member(navigationName);

                if (navEntry is CollectionEntry collectionEntry)
                {
                    await collectionEntry.LoadAsync();

                    var items = (IEnumerable<object>)collectionEntry.CurrentValue!;
                    foreach (var item in items.ToList())
                        _context.Remove(item);
                }
                else if (navEntry is ReferenceEntry referenceEntry)
                {
                    await referenceEntry.LoadAsync();

                    if (referenceEntry.CurrentValue != null)
                        _context.Remove(referenceEntry.CurrentValue);
                }
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> predicate) => 
            await _dbSet.AnyAsync(predicate);
    }
}
