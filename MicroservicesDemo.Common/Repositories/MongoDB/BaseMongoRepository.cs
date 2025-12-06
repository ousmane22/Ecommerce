#nullable disable
using Ecommerce.Common.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Ecommerce.Common.Repositories.MongoDB;

public abstract class BaseMongoRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly IMongoCollection<TEntity> Collection;
    protected readonly ILogger<BaseMongoRepository<TEntity>> Logger;

    protected BaseMongoRepository(
        IMongoCollection<TEntity> collection,
        ILogger<BaseMongoRepository<TEntity>> logger)
    {
        Collection = collection;
        Logger = logger;
    }

    public virtual async Task<TEntity> GetByIdAsync(object id)
    {
        try
        {
            var filter = BuildIdFilter(id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la récupération de l'entité avec l'ID {Id}", id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await Collection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la récupération de toutes les entités");
            throw;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await Collection.Find(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la recherche d'entités");
            throw;
        }
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await Collection.Find(predicate).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la recherche de la première entité");
            throw;
        }
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        try
        {
            await Collection.InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de l'ajout de l'entité");
            throw;
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        try
        {
            await Collection.InsertManyAsync(entities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de l'ajout de plusieurs entités");
            throw;
        }
    }

    public virtual void Update(TEntity entity)
    {
        try
        {
            var id = GetEntityId(entity);
            var filter = BuildIdFilter(id);
            Collection.ReplaceOne(filter, entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la mise à jour de l'entité");
            throw;
        }
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        try
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la mise à jour de plusieurs entités");
            throw;
        }
    }

    public virtual void Remove(TEntity entity)
    {
        try
        {
            var id = GetEntityId(entity);
            var filter = BuildIdFilter(id);
            Collection.DeleteOne(filter);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la suppression de l'entité");
            throw;
        }
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        try
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la suppression de plusieurs entités");
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var count = await Collection.CountDocumentsAsync(predicate);
            return count > 0;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de la vérification de l'existence de l'entité");
            throw;
        }
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        try
        {
            if (predicate == null)
            {
                return (int)await Collection.CountDocumentsAsync(_ => true);
            }
            return (int)await Collection.CountDocumentsAsync(predicate);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors du comptage des entités");
            throw;
        }
    }

    protected abstract FilterDefinition<TEntity> BuildIdFilter(object id);
    protected abstract object GetEntityId(TEntity entity);
}

