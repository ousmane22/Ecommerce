using MediatR;
using PaymentService.Domain.Repositories;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetPaymentsByCustomerQueryHandler : IRequestHandler<GetPaymentsByCustomerQuery, IEnumerable<PaymentDto>>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentsByCustomerQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var payments = await _repository.GetByCustomerIdAsync(request.CustomerId);
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            OrderId = p.OrderId,
            CustomerId = p.CustomerId,
            Amount = p.Amount,
            Currency = p.Currency,
            Status = p.Status.ToString(),
            Method = p.Method.ToString(),
            TransactionId = p.TransactionId,
            CreatedAt = p.CreatedAt
        });
    }
}
