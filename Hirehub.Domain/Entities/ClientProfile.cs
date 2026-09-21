using Hirehub.Domain.Common;

namespace Hirehub.Domain.Entities;

public class ClientProfile : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }

    public ICollection<Category> PreferredCategories { get; set; } = new List<Category>();
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}