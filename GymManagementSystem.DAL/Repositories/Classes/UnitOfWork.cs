using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories;

        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<string, object>();
            SessionRepository = sessionRepository;
            MembershipRepository = membershipRepository;
            BookingRepository = bookingRepository;

        }

        public ISessionRepository SessionRepository { get; }

        public IMembershipRepository MembershipRepository { get; }

        public IBookingRepository BookingRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var entityName = typeof(TEntity).Name;
            if (_repositories.TryGetValue(entityName, out object? repository))
                return (IGenericRepository<TEntity>)repository;
            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[entityName] = repo;
                return repo;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
