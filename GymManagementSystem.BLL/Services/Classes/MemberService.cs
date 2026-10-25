using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Attachment;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Member> members = await _unitOfWork.GetRepository<Member>().GetAllAsync(cancellationToken: cancellationToken);
            if (!members.Any()) return Result<IEnumerable<MemberViewModel>>.NotFound("No Members Found");
            return Result<IEnumerable<MemberViewModel>>.Ok(_mapper.Map<IEnumerable<MemberViewModel>>(members));
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Email == model.Email || m.Phone == model.Phone);
            if (exists) return Result.Validation("Email or Phone already exists");

            var storedPhotoResult = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhotos", cancellationToken);
            if(string.IsNullOrWhiteSpace(storedPhotoResult.value)) return Result.Fail(storedPhotoResult.error);

            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoResult.value;

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result > 0)
                return Result.Ok();
            else
            {
                _attachmentService.DeleteAttachment(storedPhotoResult.value, "MembersPhotos");
                return Result.Fail("Failed to Create Member");
            }
        }

        public async Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);
            if (member == null) return Result<MemberViewModel?>.NotFound("Member Not Found");

            var activeMembership = await _unitOfWork.GetRepository<Membership>().GetFirstOrDefaultAsync(m => m.MemberId == id && m.EndDate > DateTime.Now, cancellationToken: cancellationToken);
            var model = _mapper.Map<MemberViewModel>(member);

            if (activeMembership != null)
            {
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, cancellationToken);
                model.PlanName = plan?.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                model.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }
            return Result<MemberViewModel?>.Ok(model);
        }

        public async Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().GetFirstOrDefaultAsync(hr => hr.MemberId == id, cancellationToken: cancellationToken);
            if (healthRecord == null) return Result<HealthRecordViewModel?>.NotFound("Health Record Not Found");
            return Result<HealthRecordViewModel?>.Ok(_mapper.Map<HealthRecordViewModel>(healthRecord));
        }

        public async Task<Result<MemberToBeUpdatedViewModel?>> GetMemberToUpdateAsync(int id, CancellationToken cancellationToken = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);
            if (member == null) return Result<MemberToBeUpdatedViewModel?>.NotFound("Member Not Found");
            return Result<MemberToBeUpdatedViewModel?>.Ok(_mapper.Map<MemberToBeUpdatedViewModel>(member));
        }

        public async Task<Result> UpdateMemberAsync(int id, MemberToBeUpdatedViewModel model, CancellationToken cancellationToken = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);
            if (member == null) return Result.NotFound("Member Not Found");

            var emailExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Email == model.Email && m.Id != id, cancellationToken);
            var phoneExists = await _unitOfWork.GetRepository<Member>().ExistsAsync(m => m.Phone == model.Phone && m.Id != id, cancellationToken);
            if (emailExists || phoneExists) return Result.Validation("Email or Phone already in use by another member");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Member");
        }

        public async Task<Result> DeleteMemberAsync(int id, CancellationToken cancellationToken = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);
            if (member == null) return Result.NotFound("Member Not Found");

            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().ExistsAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now);
            if (hasFutureBookings) return Result.Fail("Cannot Delete Member with Future Bookings");

            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Member");
        }
    }
}
