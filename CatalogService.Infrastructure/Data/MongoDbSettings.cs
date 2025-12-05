namespace CatalogService.Infrastructure.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "CatalogDB";
    public string ProductsCollectionName { get; set; } = "Products";
}