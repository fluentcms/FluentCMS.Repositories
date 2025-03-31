# System Patterns

## Architecture Overview

FluentCMS.Repositories implements a layered architecture with clear separation of concerns:

```mermaid
graph TD
    A[Application Layer] --> B[Repository Interfaces]
    B --> C[Database-Specific Implementations]
    C --> D[Actual Databases]
    
    subgraph Repositories
        B
        C
    end
```

## Core Design Patterns

### Repository Pattern

The core pattern is the Repository Pattern, which:
- Mediates between the domain and data mapping layers
- Provides a collection-like interface for domain objects
- Hides query complexity and data access details from business logic

Implementation approach:
- Generic interfaces defining standard operations
- Database-specific implementations of these interfaces
- Query objects for complex data retrieval

### Generic Repository

```mermaid
classDiagram
    class IBaseEntityRepository~T~ {
        +GetById(Guid id) Task~T~
        +GetAll() Task~IEnumerable~T~~
        +Query(Expression filter, SortOptions sort, int skip, int take) Task~IEnumerable~T~~
        +Count(Expression filter) Task~int~
        +Add(T entity) Task
        +AddMany(IEnumerable~T~ entities) Task
        +Update(T entity) Task
        +Delete(Guid id) Task
        +DeleteMany(Expression filter) Task
    }
    
    class MongoDbRepository~T~ {
        -IMongoCollection~T~ _collection
        +MongoDbRepository(IMongoDatabase db)
    }
    
    class SqlServerRepository~T~ {
        -DbContext _context
        -DbSet~T~ _entities
        +SqlServerRepository(DbContext context)
    }
    
    class SqliteRepository~T~ {
        -DbContext _context
        -DbSet~T~ _entities
        +SqliteRepository(DbContext context)
    }
    
    IBaseEntityRepository~T~ <|.. MongoDbRepository~T~
    IBaseEntityRepository~T~ <|.. SqlServerRepository~T~
    IBaseEntityRepository~T~ <|.. SqliteRepository~T~
```

### Query Specification Pattern

For more complex queries, a specification pattern can be implemented:

```mermaid
classDiagram
    class IQuerySpecification~T~ {
        +Expression~Func~T, bool~~ Criteria
        +List~Expression~Func~T, object~~~ Includes
        +List~string~ IncludeStrings
        +Expression~Func~T, object~~ OrderBy
        +Expression~Func~T, object~~ OrderByDescending
        +int Take
        +int Skip
        +bool IsPagingEnabled
    }
    
    class BaseSpecification~T~ {
        +Expression~Func~T, bool~~ Criteria
        +List~Expression~Func~T, object~~~ Includes
        +List~string~ IncludeStrings
        +Expression~Func~T, object~~ OrderBy
        +Expression~Func~T, object~~ OrderByDescending
        +int Take
        +int Skip
        +bool IsPagingEnabled
        +AddInclude(Expression include)
        +AddInclude(string includeString)
        +ApplyPaging(int skip, int take)
        +ApplyOrderBy(Expression orderBy)
        +ApplyOrderByDescending(Expression orderByDescending)
    }
    
    IQuerySpecification~T~ <|.. BaseSpecification~T~
```

### Options Pattern

For configuration:

```mermaid
classDiagram
    class MongoDbOptions {
        +string ConnectionString
        +string DatabaseName
    }
    
    class SqlServerOptions {
        +string ConnectionString
    }
    
    class SqliteOptions {
        +string ConnectionString
    }
    
    class RepositoryOptions {
        +MongoDbOptions MongoDb
        +SqlServerOptions SqlServer
        +SqliteOptions Sqlite
    }
```

## Extension Methods for DI Registration

```csharp
// Example pattern for service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, Action<RepositoryOptions> configureOptions)
    {
        // Configure options
        services.Configure(configureOptions);
        
        // Register common services
        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(ConcreteRepositoryImplementation<>));
        
        return services;
    }
    
    public static IServiceCollection AddMongoDbRepositories(this IServiceCollection services, Action<MongoDbOptions> configureOptions)
    {
        // Configure MongoDB options
        services.Configure(configureOptions);
        
        // Register MongoDB specific services
        services.AddSingleton<IMongoClient>(sp => {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });
        
        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(MongoDbRepository<>));
        
        return services;
    }
    
    // Similar methods for SQL Server and SQLite
}
```

## Data Flow

```mermaid
sequenceDiagram
    participant A as Application
    participant R as Repository
    participant D as Database
    
    A->>R: Request data (GetById, Query, etc.)
    R->>D: Translate to database-specific query
    D->>R: Return raw data
    R->>A: Map to domain entities and return
    
    A->>R: Save data (Add, Update)
    R->>D: Translate to database-specific commands
    D->>R: Confirm operation
    R->>A: Return result
```

## Key Implementation Details

1. **Entity Interface**:
   ```csharp
   public interface IBaseEntity
   {
       Guid Id { get; set; }
   }
   ```

2. **Expression Translation**: Each database implementation will need to translate LINQ expressions to its native query language (MongoDB expressions, SQL, etc.)

3. **Pagination Handling**: Consistent skip/take implementation across all providers

4. **Error Handling**: Standardized approach to database errors and conversion to meaningful exceptions

5. **Performance Considerations**: 
   - Efficient query generation
   - Proper use of indexes
   - Minimizing unnecessary round trips to the database
