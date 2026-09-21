using Hirehub.Domain.Common;

namespace Hirehub.Domain.Entities;

public class Service : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public bool IsActive { get; set; } = true;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}