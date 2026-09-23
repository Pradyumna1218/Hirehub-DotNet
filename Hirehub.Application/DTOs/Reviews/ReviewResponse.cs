namespace Hirehub.Application.DTOs.Reviews;

public class ReviewResponse
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string FreelancerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}