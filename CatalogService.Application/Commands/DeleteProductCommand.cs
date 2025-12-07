using MediatR;

namespace CatalogService.Application.Commands;
public class DeleteProductCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
}