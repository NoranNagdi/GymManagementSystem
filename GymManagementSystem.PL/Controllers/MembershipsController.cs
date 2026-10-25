using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<ActionResult> Index(CancellationToken cancellationToken = default!)
        {
            var result = await _membershipService.GetAllMembershipsAsync(cancellationToken);
            return View(result.value);
        }

        #region Create Membership

        public async Task<IActionResult> Create(CancellationToken cancellationToken = default!)
        {
            await PopulateDropDownAsync(cancellationToken);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMembership(CreateMembershipViewModel model, CancellationToken cancellationToken = default!)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(cancellationToken);
                return View(nameof(Create), model);
            }
            var result = await _membershipService.CreateMembershipAsync(model, cancellationToken);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                await PopulateDropDownAsync(cancellationToken);
                return View(nameof(Create), model);
            }
        }

        public async Task PopulateDropDownAsync(CancellationToken cancellationToken)
        {
            ViewBag.Members = new SelectList(await _membershipService.GetMemberDropDownSelectionAsync(cancellationToken), "Id", "Name");
            ViewBag.Plans = new SelectList(await _membershipService.GetPlanDropDownSelectionAsync(cancellationToken), "Id", "Name");
        }
        #endregion

        #region Delete Membership

        public async Task<IActionResult> Cancel([FromForm] int id, CancellationToken cancellationToken = default!)
        {
            var result = await _membershipService.DeleteMembershipAsync(id, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Membership Cancelled successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
    }
    #endregion
}
