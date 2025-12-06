# Architecture Générique et Centralisée

Ce document décrit l'architecture générique mise en place pour éviter la duplication de code (DRY) dans le projet.

## 🎯 Objectifs

- **Réutilisabilité** : Composants génériques utilisables par tous les services
- **Maintenabilité** : Code centralisé, modifications en un seul endroit
- **Cohérence** : Même structure et comportement pour tous les services
- **Productivité** : Développement plus rapide avec moins de code dupliqué

## 📦 Structure du Projet Common

### `MicroservicesDemo.Common` (Ecommerce.Common)

Projet partagé contenant tous les composants génériques :

#### 1. **Controllers** (`/Controllers`)
- `BaseController<TEntity, TKey>` : Contrôleur de base générique pour les opérations CRUD
  - Méthodes héritées : GetAll, GetById, Create, Update, Delete
  - Gestion d'erreurs centralisée
  - Documentation Swagger automatique

#### 2. **Repositories** (`/Repositories`)
- `IRepository<T>` : Interface générique pour les repositories
- `BaseMongoRepository<TEntity>` : Repository MongoDB générique
  - Implémente toutes les opérations CRUD de base
  - Méthodes abstraites pour la personnalisation

#### 3. **Extensions** (`/Extensions`)
- `SwaggerExtensions` : Configuration Swagger centralisée
  - `AddSwaggerDocumentation()` : Configuration des services
  - `UseSwaggerDocumentation()` : Configuration du pipeline
- `ServiceCollectionExtensions` : Helpers pour l'injection de dépendances
  - `AddRepository<TRepository, TInterface>()` : Enregistrement générique
  - `ConfigureSettings<TSettings>()` : Configuration centralisée

## 🔧 Utilisation

### Exemple : CatalogService

#### 1. Configuration dans `Program.cs`

```csharp
using Ecommerce.Common.Extensions;

// Configuration Swagger centralisée
builder.Services.AddSwaggerDocumentation(
    title: "Catalog Service API",
    version: "v1",
    description: "API pour la gestion du catalogue de produits");

// Configuration MongoDB centralisée
builder.Services.ConfigureSettings<MongoDbSettings>(
    builder.Configuration,
    "MongoDbSettings");

// Enregistrement du repository
builder.Services.AddRepository<ProductRepository, IProductRepository>();

// Pipeline
app.UseSwaggerDocumentation(
    title: "Catalog Service API",
    version: "v1",
    routePrefixEmpty: true);
```

#### 2. Repository héritant de `BaseMongoRepository`

```csharp
public class ProductRepository : BaseMongoRepository<Product>, IProductRepository
{
    public ProductRepository(
        MongoDbContext context,
        ILogger<ProductRepository> logger) 
        : base(context.Products, logger)
    {
        _context = context;
    }

    protected override FilterDefinition<Product> BuildIdFilter(object id)
    {
        return Builders<Product>.Filter.Eq(p => p.Id, id.ToString());
    }

    protected override object GetEntityId(Product entity)
    {
        return entity.Id;
    }

    // Méthodes spécifiques au produit
    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await FindAsync(p => p.Category == category);
    }
}
```

#### 3. Contrôleur (optionnel - peut hériter de BaseController)

```csharp
public class ProductsController : BaseController<Product, string>
{
    private readonly IProductRepository _repository;

    public ProductsController(
        IProductRepository repository,
        ILogger<ProductsController> logger) : base(logger)
    {
        _repository = repository;
    }

    protected override Task<IEnumerable<Product>> GetAllEntitiesAsync()
        => _repository.GetAllAsync();

    protected override Task<Product?> GetEntityByIdAsync(string id)
        => _repository.GetByIdAsync(id);

    // ... autres méthodes abstraites
}
```

## 📋 Avantages

### ✅ Avant (Code dupliqué)
- Configuration Swagger répétée dans chaque service
- Logique CRUD dupliquée dans chaque contrôleur
- Code de repository répété pour chaque entité
- Gestion d'erreurs inconsistante

### ✅ Après (Code centralisé)
- **1 seule configuration Swagger** réutilisable
- **1 contrôleur de base** pour tous les CRUD
- **1 repository de base** pour MongoDB
- **Gestion d'erreurs uniforme** partout

## 🚀 Prochaines Étapes

1. **Refactoriser PaymentService** pour utiliser les mêmes composants
2. **Ajouter des helpers** pour la validation (FluentValidation)
3. **Créer des DTOs de base** pour les réponses API
4. **Centraliser la configuration** des middlewares (CORS, Authentication, etc.)

## 📝 Notes

- Les méthodes spécifiques peuvent toujours être ajoutées dans les classes dérivées
- Le pattern Template Method est utilisé pour la personnalisation
- Tous les composants sont testables et mockables



