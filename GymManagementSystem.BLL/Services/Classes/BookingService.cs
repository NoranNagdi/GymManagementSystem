using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.BookingsViewModel;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<BookingViewModel>>> GetSessionBookingsAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsForSession(sessionId, cancellationToken);
            if (bookings == null || !bookings.Any()) return Result<IEnumerable<BookingViewModel>>.NotFound("There are no Bookings for this Session");
            return Result<IEnumerable<BookingViewModel>>.Ok(_mapper.Map<IEnumerable<BookingViewModel>>(bookings));

        }

        public async Task<IEnumerable<MemberSelectionViewModel>> GetAvailableMembersDropDownAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var bookings = (await _unitOfWork.BookingRepository.GetAllAsync(cancellationToken: cancellationToken)).Where(b => b.SessionId == sessionId);
            var membersWithBookings = bookings.Select(b => b.MemberId);
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(cancellationToken: cancellationToken);
            var availableMembers = members.Where(m => !membersWithBookings.Contains(m.Id));
            return _mapper.Map<IEnumerable<MemberSelectionViewModel>>(availableMembers);

        }
        public async Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken cancellationToken = default)
        {
            var bookingExists = await _unitOfWork.BookingRepository.ExistsAsync(b => b.MemberId == model.MemberId && b.SessionId == model.SessionId, cancellationToken);
            if (bookingExists) return Result.Validation("There is already booking exsists for this member");

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(model.SessionId, cancellationToken);
            if (session is null) return Result.NotFound("Session was not found");

            if (!(session.StartDate > DateTime.Now)) return Result.Validation("Cannot book a session that has already started. ");

            var membershipExists = await _unitOfWork.GetRepository<Membership>().ExistsAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, cancellationToken);
            if (!membershipExists) return Result.Validation("There is No Active Membership for this member");

            var bookedSlots = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, cancellationToken);
            if (bookedSlots >= session.Capacity) return Result.Validation("The Session is Full");

            var booking = _mapper.Map<Booking>(model);
            booking.IsAttended = false;
            booking.CreatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Booking>().Add(booking);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Booking");
        }

        public async Task<Result> CancelBookingAsync(int sessionId, int memebrId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(sessionId, cancellationToken);
            if (session is null) return Result.NotFound("Session was not found");

            if (!(session.StartDate > DateTime.Now)) return Result.Validation("Cannot Cancel a session that has already started. ");

            var booking = await _unitOfWork.BookingRepository.GetFirstOrDefaultAsync(b => b.MemberId == memebrId && b.SessionId == sessionId, true, cancellationToken);
            if (booking is null) return Result.NotFound("There is No booking exsists for this member on this session");


            _unitOfWork.BookingRepository.Delete(booking);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Cancel Booking");

        }

        public async Task<Result> MarkMemberAsAttendedAsync(int sessionId, int memebrId, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(sessionId, cancellationToken);
            if (session is null) return Result.NotFound("Session was not found");

            if ((session.EndDate < DateTime.Now)) return Result.Validation("Cannot Edit a session that has already Ended. ");

            var booking = await _unitOfWork.BookingRepository.GetFirstOrDefaultAsync(b => b.MemberId == memebrId && b.SessionId == sessionId, true, cancellationToken);
            if (booking is null) return Result.NotFound("There is No booking exsists for this member on this session");

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.BookingRepository.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Booking");

        }
    }
}
