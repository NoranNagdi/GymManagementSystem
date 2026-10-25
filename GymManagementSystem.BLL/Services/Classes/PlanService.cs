using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken cancellationToken = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(cancellationToken: cancellationToken);
            if (!plans.Any()) return Result<IEnumerable<PlanViewModel>>.NotFound("No Plans Found");
            return Result<IEnumerable<PlanViewModel>>.Ok(_mapper.Map<IEnumerable<PlanViewModel>>(plans));
        }

        public async Task<Result<PlanViewModel?>> GetPlanDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, cancellationToken);
            if (plan == null) return Result<PlanViewModel?>.NotFound("Plan Not Found");
            return Result<PlanViewModel?>.Ok(_mapper.Map<PlanViewModel>(plan));
        }

        public async Task<Result<PlanToBeUpdatedViewModel?>> GetPlanToUpdateAsync(int id, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, cancellationToken);
            if (plan == null) return Result<PlanToBeUpdatedViewModel?>.NotFound("Plan Not Found");
            if (!plan.IsActive) return Result<PlanToBeUpdatedViewModel?>.Fail("Cannot edit an inactive plan");
            if (await HasActiveMembershipsAsync(id, cancellationToken)) return Result<PlanToBeUpdatedViewModel?>.Fail("Cannot edit a plan with active memberships");

            return Result<PlanToBeUpdatedViewModel?>.Ok(_mapper.Map<PlanToBeUpdatedViewModel>(plan));
        }

        public async Task<Result> UpdatePlanAsync(int id, PlanToBeUpdatedViewModel planToBeUpdatedViewModel, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, cancellationToken);
            if (plan == null) return Result.NotFound("Plan Not Found");
            if (await HasActiveMembershipsAsync(id, cancellationToken)) return Result.Fail("Cannot update a plan with active memberships");

            _mapper.Map(planToBeUpdatedViewModel, plan);
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Plan");
        }

        public async Task<Result> TogglePlanStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, cancellationToken);
            if (plan == null) return Result.NotFound("Plan Not Found");
            if (await HasActiveMembershipsAsync(id, cancellationToken)) return Result.Fail("Cannot toggle status of a plan with active memberships");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Toggle Plan Status");
        }

        #region Helper Methods
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.GetRepository<Membership>().ExistsAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, cancellationToken);
        }
        #endregion
    }
}
