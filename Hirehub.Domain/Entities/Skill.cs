using Hirehub.Domain.Common;

namespace Hirehub.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();
}