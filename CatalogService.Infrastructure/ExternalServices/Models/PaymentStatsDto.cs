namespace CatalogService.Infrastructure.ExternalServices.Models;

public class PaymentStatsDto
{
    public string ProductId { get; set; } = string.Empty;
    public int TotalPayments { get; set; }
    public decimal TotalRevenue { get; set; }
    public int SuccessfulPayments { get; set; }
    public int FailedPayments { get; set; }
}
