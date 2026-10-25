using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.PlanViewModels
{
    public class PlanToBeUpdatedViewModel
    {
        [Required(ErrorMessage = "Plan Name Is Required")]
        [StringLength(50, ErrorMessage = "Plan Name Must Be Less Than 51")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Description Must Be Between 5 and 50")]
        public string Description { get; set; } = null!;


        [Required(ErrorMessage = "Duration Days Is Required")]
        [Range(1, 365, ErrorMessage = "Dutation Days Must Be Between 1 and 365")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price Is Required")]
        [Range(1, 10000, ErrorMessage = "Price Must Be Between 1 and 10000")]
        public decimal Price { get; set; }
    }
}
