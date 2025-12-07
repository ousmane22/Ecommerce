using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Commands;

public class UpdateProductCommand : IRequest<ProductDto>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
}