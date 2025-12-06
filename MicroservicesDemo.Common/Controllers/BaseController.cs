#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.Common.Repositories;

namespace Ecommerce.Common.Controllers;

/// <summary>
/// Contrôleur de base générique pour les opérations CRUD
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController<TEntity, TKey> : ControllerBase
    where TEntity : class
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly ILogger<BaseController<TEntity, TKey>> Logger;

    protected BaseController(
        IRepository<TEntity> repository,
        ILogger<BaseController<TEntity, TKey>> logger)
    {
        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// Récupère tous les éléments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAll()
    {
        return await HandleRequestAsync(async () =>
        {
            var entities = await Repository.GetAllAsync();
            return Ok(entities);
        });
    }

    /// <summary>
    /// Récupère un élément par son ID
    /// </summary>
    /// <param name="id">ID de l'élément</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<TEntity>> GetById(TKey id)
    {
        return await HandleRequestAsync(async () =>
        {
            var entity = await Repository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return NotFound($"Élément avec l'ID {id} introuvable.");
            }

            return Ok(entity);
        });
    }

    /// <summary>
    /// Crée un nouvel élément
    /// </summary>
    /// <param name="entity">Données de l'élément</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<TEntity>> Create([FromBody] TEntity entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return await HandleRequestAsync(async () =>
        {
            await Repository.AddAsync(entity);
            var id = GetIdFromEntity(entity);
            return CreatedAtAction(nameof(GetById), new { id }, entity);
        });
    }

    /// <summary>
    /// Met à jour un élément existant
    /// </summary>
    /// <param name="id">ID de l'élément</param>
    /// <param name="entity">Données mises à jour</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<TEntity>> Update(TKey id, [FromBody] TEntity entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return await HandleRequestAsync(async () =>
        {
            var existingEntity = await Repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound($"Élément avec l'ID {id} introuvable.");
            }

            Repository.Update(entity);
            return Ok(entity);
        });
    }

    /// <summary>
    /// Supprime un élément
    /// </summary>
    /// <param name="id">ID de l'élément</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(TKey id)
    {
        return await HandleRequestAsync(async () =>
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound($"Élément avec l'ID {id} introuvable.");
            }

            Repository.Remove(entity);
            return NoContent();
        });
    }

    protected virtual TKey GetIdFromEntity(TEntity entity)
    {
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null)
        {
            var value = idProperty.GetValue(entity);
            if (value != null)
            {
                return (TKey)Convert.ChangeType(value, typeof(TKey));
            }
        }
        throw new InvalidOperationException($"Impossible de récupérer l'ID de l'entité {typeof(TEntity).Name}");
    }

    protected async Task<ActionResult> HandleRequestAsync(Func<Task<ActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors de l'exécution de la requête");
            return StatusCode(500, new { message = "Une erreur est survenue lors du traitement de la requête.", error = ex.Message });
        }
    }
}

