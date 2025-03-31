using System.Linq.Expressions;
using FluentCMS.Repositories.Abstractions.Entities;
using FluentCMS.Repositories.Abstractions.Exceptions;
using FluentCMS.Repositories.Abstractions.Models;
using FluentCMS.Repositories.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FluentCMS.Repositories.SqlServer.Repositories;

/// <summary>
/// SQL Server implementation of the generic repository pattern using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of entity that implements IBaseEntity.</typeparam>
public class SqlServerRepository<T> : IBaseEntityRepository<T> where T : class, IBaseEntity
{
    private readonly FluentCmsDbContext _context;
    private readonly DbSet<T> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerRepository{T}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SqlServerRepository(FluentCmsDbContext context)
    {
        _context = context;
        _entities = context.GetDbSet<T>();
    }

    /// <inheritdoc />
    public async Task<T?> GetById(Guid id)
    {
        return await _entities.FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAll()
    {
        return await _entities.ToListAsync();
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
        IQueryable<T> query = _entities;

        if (options.Filter != null)
        {
            query = query.Where(options.Filter);
        }

        if (options.Sort != null && options.Sort.Expressions.Any())
        {
            foreach (var sortExpression in options.Sort.Expressions)
            {
                if (sortExpression.Expression is Expression<Func<T, object>> expression)
                {
                    query = sortExpression.Direction == SortDirection.Ascending
                        ? query.OrderBy(expression)
                        : query.OrderByDescending(expression);
                }
            }
        }

        if (options.Skip.HasValue)
        {
            query = query.Skip(options.Skip.Value);
        }

        if (options.Take.HasValue)
        {
            query = query.Take(options.Take.Value);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> Count(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _entities;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync();
    }

    /// <inheritdoc />
    public async Task Add(T entity)
    {
        try
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
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
            await _entities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
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
            var existingEntity = await _entities.FirstOrDefaultAsync(e => e.Id == entity.Id);
            
            if (existingEntity == null)
            {
                throw new EntityNotFoundException(entity.Id, typeof(T).Name);
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
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
            var entity = await _entities.FirstOrDefaultAsync(e => e.Id == id);
            
            if (entity == null)
            {
                throw new EntityNotFoundException(id, typeof(T).Name);
            }

            _entities.Remove(entity);
            await _context.SaveChangesAsync();
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
            var entities = await _entities.Where(filter).ToListAsync();
            _entities.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("DeleteMany", ex);
        }
    }
}
