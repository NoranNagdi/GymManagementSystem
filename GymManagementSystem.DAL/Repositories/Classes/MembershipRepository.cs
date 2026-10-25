using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly GymDbContext _dbContext;

        public MembershipRepository(GymDbContext dbContext) : base(dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Membership>> GetActiveMembershipsWithMemberAndPlanAsync(CancellationToken cancellationToken = default)
        {
            var memberships = await _dbContext.Memberships.Where(m=>m.EndDate > DateTime.Now).AsNoTracking().Include(m => m.Member).Include(m => m.Plan).ToListAsync(cancellationToken);
            return memberships;
        }

    }
}
