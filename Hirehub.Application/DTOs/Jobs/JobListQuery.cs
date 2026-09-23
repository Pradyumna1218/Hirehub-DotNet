namespace Hirehub.Application.DTOs.Jobs;

public class JobListQuery
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}