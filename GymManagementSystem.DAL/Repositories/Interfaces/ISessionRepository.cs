using GymManagementSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainersAndCategoryAsync(CancellationToken cancellationToken = default);
        Task<int> GetCountOfBookedSlotsAsync(int id, CancellationToken cancellationToken = default);
        Task<Session?> GetSessionDetailsWithTrainerAndCategoryAsync(int id, CancellationToken cancellationToken = default);
    }
}
