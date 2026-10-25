using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken cancellationToken = default)
        {
            var memberExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Id == model.MemberId, cancellationToken);
            if (!memberExists) return Result.NotFound("Member was Not Found");

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId,cancellationToken);
            if (plan is null) return Result.NotFound("Plan was not found");
            if (!plan.IsActive) return Result.Validation("This plan is currently inactive");

            var membershipExists = await _unitOfWork.GetRepository<Membership>().ExistsAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, cancellationToken);
            if (membershipExists) return Result.Validation("There is already Active Membership for this member");

            var membership = _mapper.Map<Membership>(model);
            membership.CreatedAt = DateTime.UtcNow;
            membership.EndDate = membership.CreatedAt.AddDays(plan.DurationDays);
            _unitOfWork.GetRepository<Membership>().Add(membership);
            
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Membership");
        }

        public async Task<Result> DeleteMembershipAsync(int id, CancellationToken cancellationToken = default)
        {
            //var membership = await _unitOfWork.GetRepository<Membership>().GetFirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now, cancellationToken:cancellationToken);
            var membership = await _unitOfWork.GetRepository<Membership>().GetByIdAsync(id, cancellationToken);
            if (membership is null) return Result.NotFound("This Membership was Not Found");
            if (!membership.IsActive) return Result.Fail("Cannot Cancel Inactive Membership");

            _unitOfWork.GetRepository<Membership>().Delete(membership);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Cancel Membership");
        }

        public async Task<Result<IEnumerable<MembershipViewModel>>> GetAllMembershipsAsync(CancellationToken cancellationToken = default)
        {
            var memberships = await _unitOfWork.MembershipRepository.GetActiveMembershipsWithMemberAndPlanAsync(cancellationToken);
            //var memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync(cancellationToken: cancellationToken);
            if (!memberships.Any()) return Result<IEnumerable<MembershipViewModel>>.NotFound("No Memberships Have been Found !");
            return Result<IEnumerable<MembershipViewModel>>.Ok(_mapper.Map<IEnumerable<MembershipViewModel>>(memberships));
        }

        public async Task<IEnumerable<MemberSelectionViewModel>> GetMemberDropDownSelectionAsync(CancellationToken cancellationToken = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(cancellationToken:cancellationToken);
            return _mapper.Map<IEnumerable<MemberSelectionViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectionViewModel>> GetPlanDropDownSelectionAsync(CancellationToken cancellationToken = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<PlanSelectionViewModel>>(plans);
        }
    }
}
