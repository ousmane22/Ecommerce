using System;

namespace CatalogService.Domain.Events
{
    public class ProductDeletedEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
