using Hirehub.Domain.Common;
using Hirehub.Domain.Enums;

namespace Hirehub.Domain.Entities;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;
    public PaymentProvider Provider { get; set; } = PaymentProvider.Khalti;
    public string? ProviderToken { get; set; }
    public string? ProviderTransactionId { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? PaidAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public string PayerUserId { get; set; } = string.Empty;

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}