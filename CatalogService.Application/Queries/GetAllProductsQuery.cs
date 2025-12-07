using MediatR;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}
