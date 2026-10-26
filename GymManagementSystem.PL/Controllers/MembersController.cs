using GymManagementSystem.BLL.Services.Attachment;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var result = await _memberService.GetAllMembersAsync(cancellationToken);
            return View(result.value);
        }

        public async Task<IActionResult> GetPhoto(int id)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id);
            if (result.value is null || string.IsNullOrWhiteSpace(result.value.Photo)) return NotFound();

            var attachment = _attachmentService.GetAttachment(result.value.Photo, "MembersPhotos");

            if (attachment is null) return NotFound();

            return File(attachment.Value.stream, attachment.Value.contentType);
        }

        #region CreateMember

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            else
            {
                var result = await _memberService.CreateMemberAsync(model, cancellationToken);
                if (result.success)
                    TempData["SuccessMessage"] = "Member Created successfully.";
                else
                    TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        public async Task<IActionResult> MemberDetails(int id, CancellationToken cancellationToken = default)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken cancellationToken = default)
        {
            var result = await _memberService.GetMemberHealthRecordDetailsByIdAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        #region EditMember

        public async Task<IActionResult> EditMember(int id, CancellationToken cancellationToken = default)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToBeUpdatedViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var result = await _memberService.UpdateMemberAsync(id, model, cancellationToken);
                if (result.success)
                    TempData["SuccessMessage"] = "Member Updated successfully.";
                else
                    TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region DeleteMember

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, cancellationToken);
            if (result.success)
                return View();
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _memberService.DeleteMemberAsync(id, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Member Deleted successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
