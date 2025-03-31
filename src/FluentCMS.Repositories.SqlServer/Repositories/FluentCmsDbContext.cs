using FluentCMS.Repositories.Abstractions.Entities;
using FluentCMS.Repositories.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FluentCMS.Repositories.SqlServer.Repositories;

/// <summary>
/// Base DbContext for SQL Server repositories.
/// </summary>
public class FluentCmsDbContext : DbContext
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentCmsDbContext"/> class.
    /// </summary>
    /// <param name="options">The SQL Server options.</param>
    public FluentCmsDbContext(SqlServerOptions options)
    {
        _connectionString = options.ConnectionString;
    }

    /// <summary>
    /// Configures the database connection.
    /// </summary>
    /// <param name="optionsBuilder">The options builder.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    /// <summary>
    /// Gets a DbSet for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type that implements IBaseEntity.</typeparam>
    /// <returns>A DbSet that can be used to query and save instances of T.</returns>
    public DbSet<T> GetDbSet<T>() where T : class, IBaseEntity
    {
        return Set<T>();
    }

    /// <summary>
    /// Configures the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure entities
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(IBaseEntity).IsAssignableFrom(t.ClrType));
        
        foreach (var entityType in entityTypes)
        {
            // Set Id as primary key for all entities
            modelBuilder.Entity(entityType.ClrType)
                .HasKey(nameof(IBaseEntity.Id));
        }
    }
}
