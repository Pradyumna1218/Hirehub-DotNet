namespace Hirehub.Application.DTOs.Jobs;

public class JobCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime Deadline { get; set; }
    public int CategoryId { get; set; }
}