using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class RepositoryBaseAsync<T, K, TContext> : IRepositoryBaseAsync<T, K, TContext>
        where T : EntityBase<K>
        where TContext : DbContext
    {

        private readonly TContext _context;
        private readonly IUnitOfWork<TContext> _unitOfWork;

        public RepositoryBaseAsync(TContext context, IUnitOfWork<TContext> unitOfWork)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }

        public async Task<K> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity.Id;
        }

        public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities.Select(x => x.Id).ToList();
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteListAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task EndTransactionAsync()
        {
            await SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }

        public IQueryable<T> FindAll(bool trackChanges = false)
        {
            return !trackChanges ? _context.Set<T>().AsNoTracking() : _context.Set<T>();
        }

        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = this.FindAll(trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges = false)
        {
            return !trackChanges ? _context.Set<T>().Where(condition).AsNoTracking() : _context.Set<T>().Where(condition);
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = this.FindByCondition(condition, trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public async Task<T?> GetByIdAsync(K id)
        {
            return await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
        {
            return await FindByCondition(x => x.Id.Equals(id), false, includeProperties).FirstOrDefaultAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return _unitOfWork.CommmitAsync();
        }

        public Task UpdateAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Unchanged) return Task.CompletedTask;

            T exist = _context.Set<T>().Find(entity.Id);
            _context.Entry(exist).CurrentValues.SetValues(entity);

            return Task.CompletedTask;
        }

        public Task UpdateListAsync(IEnumerable<T> entities)
        {
            return _context.Set<T>().AddRangeAsync(entities);
        }
    }
}
