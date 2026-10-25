using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;

        public GenericRepository(GymDbContext gymDbContext)
        {
            _dbContext = gymDbContext;
            _set = _dbContext.Set<TEntity>();
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _set.FindAsync(id, cancellationToken);

        }

        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _set.AnyAsync(predicate);
        }

        public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken cancellationToken = default)
        {
            return tracking ? _set.FirstOrDefaultAsync(predicate) : _set.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                return await _set.AsNoTracking().CountAsync(cancellationToken);
            }
            return await _set.AsNoTracking().CountAsync(predicate, cancellationToken);
        }
    }
}