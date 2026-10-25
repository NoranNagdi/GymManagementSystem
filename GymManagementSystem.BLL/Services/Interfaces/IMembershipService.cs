using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<Result<IEnumerable<MembershipViewModel>>> GetAllMembershipsAsync(CancellationToken cancellationToken = default!);
        Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteMembershipAsync(int id, CancellationToken cancellationToken = default!);
        Task<IEnumerable<MemberSelectionViewModel>> GetMemberDropDownSelectionAsync(CancellationToken cancellationToken = default!);
        Task<IEnumerable<PlanSelectionViewModel>> GetPlanDropDownSelectionAsync(CancellationToken cancellationToken = default!);
    }
}
