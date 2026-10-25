using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetAllSessionsAsync(cancellationToken);
            return View(result.value);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetSessionDetailsAsync(id, cancellationToken);
            if (result.success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #region CreateSession 
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PopulateDropDownAsync(cancellationToken);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(cancellationToken);
                return View(model);
            }
            else
            {
                var result = await _sessionService.CreateSessionAsync(model, cancellationToken);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Session Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.error;
                    await PopulateDropDownAsync(cancellationToken);
                    return View(model);
                }
            }
        }
        public async Task PopulateDropDownAsync(CancellationToken cancellationToken)
        {
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoryDropDownSelectionsAsync(cancellationToken), "Id", "Name");
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerDropDownSelectionsAsync(cancellationToken), "Id", "Name");
        }
        #endregion

        #region EditSession

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, cancellationToken);
            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerDropDownSelectionsAsync(), "Id", "Name");
                return View(result.value);

            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));

            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SessionToBeUpdatedViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerDropDownSelectionsAsync(), "Id", "Name");
                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, cancellationToken);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully ";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerDropDownSelectionsAsync(), "Id", "Name");
                return View(model);
            }
        }

        #endregion

        #region DeleteSession

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetSessionDetailsAsync(id, cancellationToken);
            if (result.success) return View();
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.DeleteSessionAsync(id, cancellationToken);

            if (result.success)
                TempData["SuccessMessage"] = "Session Deleted Successfully";

            else
                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
