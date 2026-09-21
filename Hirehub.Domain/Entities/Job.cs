using Hirehub.Domain.Common;
using Hirehub.Domain.Enums;

namespace Hirehub.Domain.Entities;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime Deadline { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Open;

    public int ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}