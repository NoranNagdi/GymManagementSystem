using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.BookingsViewModel;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    public class BookingController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IBookingService _bookingService;

        public BookingController(ISessionService sessionService, IBookingService bookingService)
        {
            _sessionService = sessionService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default!)
        {
            var result = await _sessionService.GetAllSessionsAsync(cancellationToken);
            return View(result.value);
        }
        public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken cancellationToken = default!)
        {
            var result = await _bookingService.GetSessionBookingsAsync(id, cancellationToken);
            return View(result.value);
        }
        public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken cancellationToken = default!)
        {
            var result = await _bookingService.GetSessionBookingsAsync(id, cancellationToken);
            return View(result.value);
        }

        public async Task<IActionResult> Create(int id, CancellationToken cancellationToken = default)
        {
            await PopulateDropDownAsync(id, cancellationToken);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingViewModel model, CancellationToken cancellationToken = default!)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(model.SessionId);
                return View(nameof(Create), model);
            }
            var result = await _bookingService.CreateBookingAsync(model, cancellationToken);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Booking Created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                await PopulateDropDownAsync(model.SessionId, cancellationToken);
            }
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
        }

        public async Task PopulateDropDownAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            ViewBag.Members = new SelectList(await _bookingService.GetAvailableMembersDropDownAsync(sessionId, cancellationToken), "Id", "Name");
            ViewBag.SessionId = sessionId;
        }

        public async Task<IActionResult> Cancel(int sessionId, int memberId, CancellationToken cancellationToken)
        {
            var result = await _bookingService.CancelBookingAsync(sessionId, memberId, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Booking Cancelled successfully.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
        }
        public async Task<IActionResult> Attended(int sessionId, int memberId, CancellationToken cancellationToken)
        {
            var result = await _bookingService.MarkMemberAsAttendedAsync(sessionId, memberId, cancellationToken);
            if (result.success)
                TempData["SuccessMessage"] = "Member Marked As Attended.";
            else
                TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
        }

    }
}
