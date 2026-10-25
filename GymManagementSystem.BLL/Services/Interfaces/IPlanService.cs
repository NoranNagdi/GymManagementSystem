using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken cancellationToken = default);
        Task<Result<PlanViewModel?>> GetPlanDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<PlanToBeUpdatedViewModel?>> GetPlanToUpdateAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> UpdatePlanAsync(int id, PlanToBeUpdatedViewModel planToBeUpdatedViewModel, CancellationToken cancellationToken = default);
        Task<Result> TogglePlanStatusAsync(int id, CancellationToken cancellationToken = default);
    }
}
