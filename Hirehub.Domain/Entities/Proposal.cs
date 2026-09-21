using Hirehub.Domain.Common;
using Hirehub.Domain.Enums;

namespace Hirehub.Domain.Entities;

public class Proposal : BaseEntity
{
    public string CoverLetter { get; set; } = string.Empty;
    public decimal ProposedPrice { get; set; }
    public int DeliveryDays { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;

    public Order? Order { get; set; }
}