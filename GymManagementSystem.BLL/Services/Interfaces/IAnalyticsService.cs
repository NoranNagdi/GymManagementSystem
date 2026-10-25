using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Result<AnalyticsViewModel>> GetDashboardAnalyticsAsync(CancellationToken cancellationToken = default);
        //Task<Result<int>> GetTotalMembersAsync();
        //Task<Result<int>> GetActiveMembersAsync();
        //Task<Result<int>> GetTotalTrainersAsync();
        //Task<Result<int>> GetUpcomingSessions();
        //Task<Result<int>> GetOngoningSessions();
        //Task<Result<int>> GetCompletedSessions();
    }
}
