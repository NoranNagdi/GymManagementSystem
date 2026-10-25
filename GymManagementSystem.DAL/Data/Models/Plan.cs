namespace GymManagementSystem.DAL.Data.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Membership> PlanMembers { get; set; } = default!;

    }
}
