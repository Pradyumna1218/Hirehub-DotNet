namespace Hirehub.Application.DTOs.Reviews;

public class ReviewCreateRequest
{
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}