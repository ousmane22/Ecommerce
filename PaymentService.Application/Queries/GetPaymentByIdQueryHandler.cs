using MediatR;
using PaymentService.Domain.Repositories;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByIdQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id);
        if (payment == null) return null!;

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            CustomerId = payment.CustomerId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            Method = payment.Method.ToString(),
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt
        };
    }
}
