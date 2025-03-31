using System.Linq.Expressions;
using FluentCMS.Repositories.Abstractions.Entities;
using FluentCMS.Repositories.Abstractions.Exceptions;
using FluentCMS.Repositories.Abstractions.Models;
using SortDirection = FluentCMS.Repositories.Abstractions.Models.SortDirection;
using FluentCMS.Repositories.Abstractions.Repositories;
using MongoDB.Driver;

namespace FluentCMS.Repositories.MongoDb.Repositories;

/// <summary>
/// MongoDB implementation of the generic repository pattern.
/// </summary>
/// <typeparam name="T">The type of entity that implements IBaseEntity.</typeparam>
public class MongoDbRepository<T> : IBaseEntityRepository<T> where T : IBaseEntity
{
    private readonly IMongoCollection<T> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbRepository{T}"/> class.
    /// </summary>
    /// <param name="database">The MongoDB database.</param>
    public MongoDbRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name);
    }

    /// <inheritdoc />
    public async Task<T?> GetById(Guid id)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAll()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> Query(
        Expression<Func<T, bool>>? filter = null,
        SortOptions<T>? sortOptions = null,
        int? skip = null,
        int? take = null)
    {
        var options = new QueryOptions<T>
        {
            Filter = filter,
            Sort = sortOptions,
            Skip = skip,
            Take = take
        };
        
        return await Query(options);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> Query(QueryOptions<T> options)
    {
        var queryFilter = options.Filter != null 
            ? Builders<T>.Filter.Where(options.Filter) 
            : Builders<T>.Filter.Empty;
        
        var query = _collection.Find(queryFilter);

        if (options.Sort != null && options.Sort.Expressions.Any())
        {
            var sortBuilder = Builders<T>.Sort;
            var sortDefinitions = new List<SortDefinition<T>>();

            foreach (var sortExpression in options.Sort.Expressions)
            {
                if (sortExpression.Expression is Expression<Func<T, object>> expression)
                {
                    var sortDefinition = sortExpression.Direction == SortDirection.Ascending
                        ? sortBuilder.Ascending(expression)
                        : sortBuilder.Descending(expression);
                    
                    sortDefinitions.Add(sortDefinition);
                }
            }

            if (sortDefinitions.Any())
            {
                query = query.Sort(sortBuilder.Combine(sortDefinitions));
            }
        }

        if (options.Skip.HasValue)
        {
            query = query.Skip(options.Skip.Value);
        }

        if (options.Take.HasValue)
        {
            query = query.Limit(options.Take.Value);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> Count(Expression<Func<T, bool>>? filter = null)
    {
        var queryFilter = filter != null 
            ? Builders<T>.Filter.Where(filter) 
            : Builders<T>.Filter.Empty;
            
        return (int)await _collection.CountDocumentsAsync(queryFilter);
    }

    /// <inheritdoc />
    public async Task Add(T entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Add", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddMany(IEnumerable<T> entities)
    {
        try
        {
            await _collection.InsertManyAsync(entities);
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("AddMany", ex);
        }
    }

    /// <inheritdoc />
    public async Task Update(T entity)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            var result = await _collection.ReplaceOneAsync(filter, entity);

            if (result.MatchedCount == 0)
            {
                throw new EntityNotFoundException(entity.Id, typeof(T).Name);
            }
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Update", ex);
        }
    }

    /// <inheritdoc />
    public async Task Delete(Guid id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new EntityNotFoundException(id, typeof(T).Name);
            }
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Delete", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteMany(Expression<Func<T, bool>> filter)
    {
        try
        {
            var mongoFilter = Builders<T>.Filter.Where(filter);
            await _collection.DeleteManyAsync(mongoFilter);
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("DeleteMany", ex);
        }
    }
}
