namespace PaymentService.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "XOF";
    public string Status { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
