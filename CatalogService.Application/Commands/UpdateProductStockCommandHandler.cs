using CatalogService.Domain.Events;
using CatalogService.Domain.Repositories;
using Ecommerce.Common.Messaging;
using MediatR;

namespace CatalogService.Application.Commands;

public class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public UpdateProductStockCommandHandler(IProductRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Produit avec l'ID {request.Id} non trouvé");
        }

        var success = await _repository.UpdateStockAsync(request.Id, request.Quantity);
        if (!success)
        {
            return false;
        }

        var stockEvent = new ProductStockUpdatedEvent
        {
            ProductId = product.Id,
            ProductName = product.Name,
            DeletedAt = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(stockEvent, "product.stock.updated");

        return true;
    }
}
