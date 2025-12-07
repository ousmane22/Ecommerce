using MediatR;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
{
    public string Category { get; set; } = string.Empty;
}
