namespace Hirehub.Application.DTOs.Orders;

public class OrderResponse
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string FreelancerName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? ServiceTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}