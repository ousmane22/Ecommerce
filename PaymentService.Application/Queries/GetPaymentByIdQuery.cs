using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetPaymentByIdQuery : IRequest<PaymentDto>
{
    public int Id { get; set; }
}
