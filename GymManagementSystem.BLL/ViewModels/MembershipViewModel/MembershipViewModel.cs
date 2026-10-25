using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.MembershipViewModel
{
    public class MembershipViewModel
    {
        public int MembershipId { get; set; }
        public string MemberName { get; set; } = default!;
        public string PlanName { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
