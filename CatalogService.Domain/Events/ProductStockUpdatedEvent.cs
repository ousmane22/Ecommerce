using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.Events
{
    public class ProductStockUpdatedEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}
