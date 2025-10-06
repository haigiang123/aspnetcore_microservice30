using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class RepositoryQueryBase<T, K, TContext> : IRepositoryQueryBase<T, K, TContext>
        where T : EntityBase<K>
        where TContext : DbContext
    {
        private readonly TContext _context;

        public RepositoryQueryBase(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
    }
}
