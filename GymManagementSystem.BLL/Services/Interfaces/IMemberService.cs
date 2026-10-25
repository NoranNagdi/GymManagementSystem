using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default);
        Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<MemberToBeUpdatedViewModel?>> GetMemberToUpdateAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> UpdateMemberAsync(int id, MemberToBeUpdatedViewModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteMemberAsync(int id, CancellationToken cancellationToken = default);
    }
}
