using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands;

public class CreatePaymentCommand : IRequest<PaymentDto>
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "XOF";
    public string Method { get; set; } = string.Empty;
}
