namespace Hirehub.Application.DTOs.Proposals;

public class ProposalCreateRequest
{
    public int JobId { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public decimal ProposedPrice { get; set; }
    public int DeliveryDays { get; set; }
}