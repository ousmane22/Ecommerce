using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetPaymentsByCustomerQuery : IRequest<IEnumerable<PaymentDto>>
{
    public string CustomerId { get; set; } = string.Empty;
}
