using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _planService.GetAllPlansAsync(cancellationToken: cancellationToken);
            if (!result.success)
            {
                return View(Enumerable.Empty<PlanViewModel>());
            }

            return View(result.value);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var result = await _planService.GetPlanDetailsAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        #region EditPlan

        public async Task<IActionResult> EditPlan(int id, CancellationToken cancellationToken)
        {
            var result = await _planService.GetPlanToUpdateAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditPlan([FromRoute] int id, PlanToBeUpdatedViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);
            else
            {
                var result = await _planService.UpdatePlanAsync(id, model, cancellationToken);
                if (result.success)
                    TempData["SuccessMessage"] = "Plan Updated successfully.";
                else
                    TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _planService.TogglePlanStatusAsync(id, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Plan Status Updated successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
    }
}
