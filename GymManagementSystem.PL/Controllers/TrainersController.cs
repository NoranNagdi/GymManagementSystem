using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _trainerService.GetAllTrainersAsync(cancellationToken);
            return View(result.value);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var result = await _trainerService.GetTrainerDetailsAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        #region CreateTrainer
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainer(CreateTrainerViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            else
            {
                var result = await _trainerService.CreateTrainerAsync(model, cancellationToken);
                if (result.success)
                    TempData["SuccessMessage"] = "Trainer Created successfully.";
                else
                    TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region EditTrainer

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var result = await _trainerService.GetTrainerToUpdateAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, TrainerToBeUpdatedViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _trainerService.UpdateTrainerAsync(id, model, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region DeleteTrainer
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _trainerService.GetTrainerDetailsAsync(id, cancellationToken);
            if (result.success)
                return View();
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var result = await _trainerService.DeleteTrainerAsync(id, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Trainer Deleted successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
