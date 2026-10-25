using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainersAndCategoryAsync(CancellationToken cancellationToken = default)
        {
            var sessions = await _dbContext.Sessions.AsNoTracking().Include(s=>s.Trainer).Include(s=>s.Category).ToListAsync(cancellationToken);
            return sessions;
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Bookings.AsNoTracking().CountAsync(b => b.SessionId == id);
        }

        public async Task<Session?> GetSessionDetailsWithTrainerAndCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            return session;
        }
    }
}
