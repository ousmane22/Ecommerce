using MediatR;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public string Id { get; set; } = string.Empty;
}
