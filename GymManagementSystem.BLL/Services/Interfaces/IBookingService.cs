using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.BookingsViewModel;
using GymManagementSystem.BLL.ViewModels.MembershipViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Result<IEnumerable<BookingViewModel>>> GetSessionBookingsAsync(int sessionId, CancellationToken cancellationToken = default!);
        Task<IEnumerable<MemberSelectionViewModel>> GetAvailableMembersDropDownAsync(int sessionId, CancellationToken cancellationToken = default!);
        Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken cancellationToken = default);
        Task<Result> CancelBookingAsync(int sessionId, int memebrId, CancellationToken cancellationToken = default);
        Task<Result> MarkMemberAsAttendedAsync(int sessionId, int memebrId, CancellationToken cancellationToken = default);



    }
}
