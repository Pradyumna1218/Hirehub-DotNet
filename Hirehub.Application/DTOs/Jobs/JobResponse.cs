namespace Hirehub.Application.DTOs.Jobs;

public class JobResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int ProposalCount { get; set; }
    public DateTime CreatedAt { get; set; }
}