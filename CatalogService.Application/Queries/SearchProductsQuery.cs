using MediatR;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class SearchProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
}
