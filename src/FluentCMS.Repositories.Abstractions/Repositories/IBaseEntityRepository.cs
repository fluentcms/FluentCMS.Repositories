using System.Linq.Expressions;
using FluentCMS.Repositories.Abstractions.Entities;
using FluentCMS.Repositories.Abstractions.Models;

namespace FluentCMS.Repositories.Abstractions.Repositories;

/// <summary>
/// Provides a generic repository interface for entity operations.
/// </summary>
/// <typeparam name="T">The type of entity that implements IBaseEntity.</typeparam>
public interface IBaseEntityRepository<T> where T : IBaseEntity
{
    // Read operations
    
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetById(Guid id);
    
    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAll();
    
    /// <summary>
    /// Queries entities with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <param name="sortOptions">The sort options.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <param name="take">The number of entities to take.</param>
    /// <returns>A collection of entities matching the query parameters.</returns>
    Task<IEnumerable<T>> Query(
        Expression<Func<T, bool>>? filter = null, 
        SortOptions<T>? sortOptions = null, 
        int? skip = null, 
        int? take = null);
    
    /// <summary>
    /// Queries entities using a QueryOptions object.
    /// </summary>
    /// <param name="options">The query options.</param>
    /// <returns>A collection of entities matching the query parameters.</returns>
    Task<IEnumerable<T>> Query(QueryOptions<T> options);
    
    /// <summary>
    /// Counts entities with an optional filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The count of entities matching the filter.</returns>
    Task<int> Count(Expression<Func<T, bool>>? filter = null);
    
    // Write operations
    
    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    Task Add(T entity);
    
    /// <summary>
    /// Adds multiple entities.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    Task AddMany(IEnumerable<T> entities);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task Update(T entity);
    
    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    Task Delete(Guid id);
    
    /// <summary>
    /// Deletes multiple entities matching a filter.
    /// </summary>
    /// <param name="filter">The filter expression to match entities for deletion.</param>
    Task DeleteMany(Expression<Func<T, bool>> filter);
}
