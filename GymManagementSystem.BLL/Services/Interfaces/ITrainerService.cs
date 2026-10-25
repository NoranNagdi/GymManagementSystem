using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken cancellationToken);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken cancellationToken);
        Task<Result<TrainerViewModel?>> GetTrainerDetailsAsync(int id, CancellationToken cancellationToken);
        Task<Result<TrainerToBeUpdatedViewModel?>> GetTrainerToUpdateAsync(int id, CancellationToken cancellationToken);
        Task<Result> UpdateTrainerAsync(int id, TrainerToBeUpdatedViewModel model, CancellationToken cancellationToken);
        Task<Result> DeleteTrainerAsync(int id, CancellationToken cancellationToken);
    }
}
