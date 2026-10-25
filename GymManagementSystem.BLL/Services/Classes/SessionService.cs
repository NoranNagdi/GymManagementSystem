using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Data.Models.Enums;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default)
        {
            if(model.StartDate >= model.EndDate) return Result.Validation("End Date Must Be After Start Date");
            if(model.StartDate < DateTime.Now) return Result.Validation("Cannot create a session with start date in the past");
            if(model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity must be between 1 and 25");
            
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, cancellationToken);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, cancellationToken);
            if (category == null) return Result.NotFound("Category Not Found");

            var validCategory = Enum.TryParse<Speciality>(category.Name,true, out var speciality);
            if(!validCategory || speciality != trainer.Speciality) return Result.Validation("The trainer speciality doesn't match the category");

            var session = _mapper.Map<Session>(model);
            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Session");
        }

        public async Task<Result<IEnumerable<SessionViewModel>>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainersAndCategoryAsync(cancellationToken: cancellationToken);
            if (sessions is null || !sessions.Any()) return Result<IEnumerable<SessionViewModel>>.NotFound("Sessions Not Found");
            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                //N+1 
                session.AvailableSlots = session.Capacity -
                    await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id);
            }
            return Result<IEnumerable<SessionViewModel>>.Ok(mappedSessions);
        }

        public async Task<Result<SessionViewModel?>> GetSessionDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionDetailsWithTrainerAndCategoryAsync(id, cancellationToken);
            if (session is null) return Result<SessionViewModel?>.NotFound("Session Not Found");

            var mappedSession = _mapper.Map<SessionViewModel>(session);
            mappedSession.AvailableSlots = mappedSession.Capacity -
                    await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id);
            return Result<SessionViewModel?>.Ok(mappedSession);
;
        }

        public async Task<IEnumerable<CategorySelectionViewModel>> GetCategoryDropDownSelectionsAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(cancellationToken:cancellationToken);
            return _mapper.Map<IEnumerable<CategorySelectionViewModel>>(categories);
        }

        public async Task<IEnumerable<TrainerSelectionViewModel>> GetTrainerDropDownSelectionsAsync(CancellationToken cancellationToken = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(cancellationToken:cancellationToken);
            return _mapper.Map<IEnumerable<TrainerSelectionViewModel>>(trainers);
        }

        public async Task<Result<SessionToBeUpdatedViewModel>> GetSessionToUpdateAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id);
            if (session == null) return Result<SessionToBeUpdatedViewModel>.NotFound("Session Not Found");
            if (session.StartDate < DateTime.Now) return Result<SessionToBeUpdatedViewModel>.Fail("Cannot edit a session with start date in the past");
            
            var bookings = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, cancellationToken);
            if (bookings > 0) return Result<SessionToBeUpdatedViewModel>.Fail("Cannot Update Session with Bookings");

            var mappedSession = _mapper.Map<SessionToBeUpdatedViewModel>(session);
            return Result<SessionToBeUpdatedViewModel>.Ok(mappedSession); 
        }

        public async Task<Result> UpdateSessionAsync(int id, SessionToBeUpdatedViewModel model, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id);
            if (session == null) return Result.NotFound("Session Not Found");
            if (session.StartDate < DateTime.Now) return Result.Fail("Cannot edit a session with start date in the past");

            var bookings = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, cancellationToken);
            if (bookings > 0) return Result.Fail("Cannot Update Session with Bookings");

            if (model.StartDate > model.EndDate) return Result.Validation("End Date Must Be After Start Date");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, cancellationToken);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, cancellationToken);
            if (category == null) return Result.NotFound("Category Not Found");

            var validCategory = Enum.TryParse<Speciality>(category.Name, true, out var speciality);
            if (!validCategory || speciality != trainer.Speciality) return Result.Validation("The trainer speciality doesn't match the category");

            _mapper.Map(model,session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Update(session);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);


            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Session");

        }

        public async Task<Result> DeleteSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id, cancellationToken);
            if(session == null) return Result.NotFound("Session Not Found");

            if (session.EndDate >= DateTime.Now) return Result.Fail("Cannot Delete Ongoing or Future Session");

             _unitOfWork.SessionRepository.Delete(session);
            var res = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return res > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session");
        }
    }
}
