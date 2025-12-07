using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetAllPaymentsQuery : IRequest<IEnumerable<PaymentDto>>
{
}
