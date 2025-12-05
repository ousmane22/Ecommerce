namespace PaymentService.Domain.Events;

public class PaymentFailedEvent
{
    public int PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; } = DateTime.UtcNow;
}