using MediatR;

namespace CatalogService.Application.Commands;

public class UpdateProductStockCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
