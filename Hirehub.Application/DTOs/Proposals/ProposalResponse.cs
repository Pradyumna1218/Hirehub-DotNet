namespace Hirehub.Application.DTOs.Proposals;

public class ProposalResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public decimal ProposedPrice { get; set; }
    public int DeliveryDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FreelancerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}