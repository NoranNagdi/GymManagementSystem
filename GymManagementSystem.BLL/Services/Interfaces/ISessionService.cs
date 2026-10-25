using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default);
        Task<Result<SessionViewModel?>> GetSessionDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategorySelectionViewModel>> GetCategoryDropDownSelectionsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainerSelectionViewModel>> GetTrainerDropDownSelectionsAsync(CancellationToken cancellationToken = default);

        Task<Result<SessionToBeUpdatedViewModel>> GetSessionToUpdateAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> UpdateSessionAsync(int id, SessionToBeUpdatedViewModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteSessionAsync(int id, CancellationToken cancellationToken = default);
    }
}
