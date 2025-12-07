using CatalogService.Domain.Events;
using CatalogService.Domain.Repositories;
using Ecommerce.Common.Messaging;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.Application.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public DeleteProductCommandHandler(
        IProductRepository repository,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Produit avec l'ID {request.Id} non trouvé");
        }

        _repository.Remove(product);

        // Publier l'événement
        var productDeletedEvent = new ProductDeletedEvent
        {
            ProductId = product.Id,
            ProductName = product.Name,
            DeletedAt = DateTime.UtcNow
        };

        await _eventPublisher.PublishAsync(productDeletedEvent, "product.deleted");

        return true;
    }
}