namespace PaymentService.Domain.Events;

public class PaymentCompletedEvent
{
    public int PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}