using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.BookingsViewModel
{
    public class BookingViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }

        public string MemberName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public bool IsAttended { get; set; }
    }
}
