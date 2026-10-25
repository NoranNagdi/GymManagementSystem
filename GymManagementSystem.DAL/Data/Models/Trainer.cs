using GymManagementSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }

        /* Hire Date will be the CreatedAt */

        public ICollection<Session> Sessions { get; set; } = default!;

    }
}
