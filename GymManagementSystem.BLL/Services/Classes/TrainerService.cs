using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken cancellationToken)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(cancellationToken: cancellationToken);
            if (!trainers.Any()) return Result<IEnumerable<TrainerViewModel>>.NotFound("No Trainers Found");
            return Result<IEnumerable<TrainerViewModel>>.Ok(_mapper.Map<IEnumerable<TrainerViewModel>>(trainers));
        }

        public async Task<Result<TrainerViewModel?>> GetTrainerDetailsAsync(int id, CancellationToken cancellationToken)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, cancellationToken);
            if (trainer == null) return Result<TrainerViewModel?>.NotFound("Trainer Not Found");
            return Result<TrainerViewModel?>.Ok(_mapper.Map<TrainerViewModel>(trainer));
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Email == model.Email || t.Phone == model.Phone);
            if (exists) return Result.Validation("Email or Phone already exists");

            var trainer = _mapper.Map<Trainer>(model);

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Trainer");
        }

        public async Task<Result<TrainerToBeUpdatedViewModel?>> GetTrainerToUpdateAsync(int id, CancellationToken cancellationToken)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, cancellationToken);
            if (trainer == null) return Result<TrainerToBeUpdatedViewModel?>.NotFound("Trainer Not Found");
            return Result<TrainerToBeUpdatedViewModel?>.Ok(_mapper.Map<TrainerToBeUpdatedViewModel>(trainer));
        }

        public async Task<Result> UpdateTrainerAsync(int id, TrainerToBeUpdatedViewModel model, CancellationToken cancellationToken)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, cancellationToken);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var emailExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Email == model.Email && t.Id != id, cancellationToken);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Phone == model.Phone && t.Id != id, cancellationToken);
            if (emailExists || phoneExists) return Result.Validation("Email or Phone already in use by another trainer");

            _mapper.Map(model, trainer);
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Trainer");
        }

        public async Task<Result> DeleteTrainerAsync(int id, CancellationToken cancellationToken)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, cancellationToken);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var hasFutureBookings = await _unitOfWork.GetRepository<Session>().ExistsAsync(s => s.TrainerId == id && s.StartDate > DateTime.Now);
            if (hasFutureBookings) return Result.Fail("Cannot Delete Trainer with Future Sessions");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Trainer");
        }
    }
}
