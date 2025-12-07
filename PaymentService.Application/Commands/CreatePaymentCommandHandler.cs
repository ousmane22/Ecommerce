using MediatR;
using PaymentService.Domain.Repositories;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using Ecommerce.Common.Messaging;

namespace PaymentService.Application.Commands;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public CreatePaymentCommandHandler(IPaymentRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = Enum.Parse<PaymentService.Domain.Enums.PaymentMethod>(request.Method),
            Status = PaymentService.Domain.Enums.PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(payment);

        await _eventPublisher.PublishAsync(new PaymentService.Domain.Events.PaymentCreatedEvent
        {
            PaymentId = created.Id,
            OrderId = created.OrderId,
            Amount = created.Amount,
            Currency = created.Currency,
            CreatedAt = created.CreatedAt
        }, "payment.created");

        return new PaymentDto
        {
            Id = created.Id,
            OrderId = created.OrderId,
            CustomerId = created.CustomerId,
            Amount = created.Amount,
            Currency = created.Currency,
            Status = created.Status.ToString(),
            Method = created.Method.ToString(),
            TransactionId = created.TransactionId,
            CreatedAt = created.CreatedAt
        };
    }
}
