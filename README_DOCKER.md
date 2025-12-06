# Guide Docker pour MongoDB

## 1. Lancer MongoDB dans un conteneur Docker

### Option A : Avec docker-compose (recommandé)

```bash
# Lancer MongoDB
docker-compose up -d

# Vérifier que le conteneur est en cours d'exécution
docker ps

# Voir les logs
docker-compose logs -f mongodb
```

### Option B : Avec docker run

```bash
docker run -d \
  --name ecommerce-mongodb \
  -p 27017:27017 \
  -e MONGO_INITDB_ROOT_USERNAME=admin \
  -e MONGO_INITDB_ROOT_PASSWORD=admin123 \
  -v mongodb_data:/data/db \
  mongo:7.0
```

## 2. Vérifier la connexion MongoDB

### Test rapide avec MongoDB Shell

```bash
# Se connecter au conteneur MongoDB
docker exec -it ecommerce-mongodb mongosh

# Dans le shell MongoDB, tester la connexion
show dbs
use CatalogDB
db.Products.find()
```

### Test avec l'application .NET

1. **Lancer l'application :**
```bash
cd CatalogService.API
dotnet run
```

2. **Tester avec Swagger :**
   - Ouvrir : `https://localhost:7208/swagger`
   - Tester les endpoints CRUD

3. **Tester avec curl :**
```bash
# Créer un produit
curl -X POST "https://localhost:7208/api/Products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Produit Test",
    "description": "Description test",
    "price": 99.99,
    "stock": 10,
    "category": "Electronics"
  }'

# Récupérer tous les produits
curl -X GET "https://localhost:7208/api/Products"
```

## 3. Arrêter MongoDB

```bash
# Arrêter le conteneur
docker-compose down

# Arrêter et supprimer les volumes (⚠️ supprime les données)
docker-compose down -v
```

## 4. Configuration de connexion

La chaîne de connexion dans `appsettings.json` est déjà configurée pour MongoDB local :
```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "CatalogDB",
  "ProductsCollectionName": "Products"
}
```

Si vous utilisez l'authentification, modifiez la chaîne de connexion :
```
mongodb://admin:admin123@localhost:27017/CatalogDB?authSource=admin
```


