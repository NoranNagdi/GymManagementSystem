using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyticsViewModel>> GetDashboardAnalyticsAsync(CancellationToken cancellationToken = default)
        {
            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(cancellationToken:cancellationToken);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(cancellationToken:cancellationToken);
            var activeMembers = await _unitOfWork.GetRepository<Membership>().CountAsync(m => m.EndDate > DateTime.Now, cancellationToken: cancellationToken);
            var ongoingSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now, cancellationToken: cancellationToken);
            var upcomingSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.StartDate > DateTime.Now, cancellationToken: cancellationToken);
            var completedSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.EndDate < DateTime.Now, cancellationToken: cancellationToken);

            var analytics = new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                OngoingSessions = ongoingSessions,
                UpcomingSessions = upcomingSessions,
                CompletedSessions = completedSessions
            };
            return Result<AnalyticsViewModel>.Ok(analytics);

        }
    }
}
