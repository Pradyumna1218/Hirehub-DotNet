using Hirehub.Domain.Common;

namespace Hirehub.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;
}