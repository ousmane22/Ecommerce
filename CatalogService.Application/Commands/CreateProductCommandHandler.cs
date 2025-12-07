using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Events;
using CatalogService.Domain.Repositories;
using Ecommerce.Common.Messaging;
using MediatR;

namespace CatalogService.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IEventPublisher eventPublisher
        )
    {
        _productRepository = repository;
        _eventPublisher = eventPublisher;
    }
    public async  Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Category = request.Category,
            ImageUrl = request.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync( product );

        // Publier l'événement
        var productCreatedEvent = new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            CreatedAt = product.CreatedAt
        };

        await _eventPublisher.PublishAsync(productCreatedEvent, "product.created");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
