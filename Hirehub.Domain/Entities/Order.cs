using Hirehub.Domain.Common;
using Hirehub.Domain.Enums;

namespace Hirehub.Domain.Entities;

public class Order : BaseEntity
{
    public decimal TotalAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public int ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;

    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public int? ProposalId { get; set; }
    public Proposal? Proposal { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Review? Review { get; set; }
}