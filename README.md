# FluentCMS.Repositories

A flexible generic repository pattern implementation supporting multiple database types.

## Overview

FluentCMS.Repositories provides a standardized way to implement data access with support for multiple database providers:

- MongoDB
- SQL Server
- SQLite

The library offers a consistent API surface while enabling database-specific optimizations behind the scenes.

## Features

- Generic repository interfaces for any entity type with a GUID identifier
- Consistent CRUD operations across all database implementations
- Flexible querying with filtering, sorting, and pagination
- Support for MongoDB, SQL Server, and SQLite
- Dependency injection support for easy integration

## Getting Started

### Prerequisites

- .NET 8.0 or later
- Connection to your preferred database (MongoDB, SQL Server, or SQLite)

### Installation

Install the NuGet packages for your preferred database provider:

```shell
# For MongoDB
dotnet add package FluentCMS.Repositories.MongoDb

# For SQL Server
dotnet add package FluentCMS.Repositories.SqlServer

# For SQLite
dotnet add package FluentCMS.Repositories.Sqlite
```

### Basic Usage

1. Create an entity class that implements `IBaseEntity`:

```csharp
using FluentCMS.Repositories.Abstractions.Entities;

public class Product : IBaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

2. Configure the repositories in your service registration:

```csharp
// MongoDB setup
services.AddMongoDbRepositories(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "YourDatabaseName";
});

// SQL Server setup
services.AddSqlServerRepositories(options =>
{
    options.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=YourDatabase;Trusted_Connection=True;";
});

// SQLite setup
services.AddSqliteRepositories(options =>
{
    options.ConnectionString = "Data Source=YourDatabase.db";
});
```

3. Inject and use the repository in your services:

```csharp
public class ProductService
{
    private readonly IBaseEntityRepository<Product> _productRepository;

    public ProductService(IBaseEntityRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        return await _productRepository.GetById(id);
    }

    public async Task<IEnumerable<Product>> GetExpensiveProducts(decimal priceThreshold)
    {
        return await _productRepository.Query(p => p.Price > priceThreshold);
    }

    public async Task AddProduct(Product product)
    {
        await _productRepository.Add(product);
    }

    public async Task UpdateProduct(Product product)
    {
        await _productRepository.Update(product);
    }

    public async Task DeleteProduct(Guid id)
    {
        await _productRepository.Delete(id);
    }
}
```

## Advanced Querying

### Filtering

```csharp
// Simple filter
var products = await _repository.Query(p => p.Price > 100);

// Complex filter
var products = await _repository.Query(
    p => p.Price > 100 && p.Name.Contains("smartphone") && p.IsAvailable
);
```

### Sorting

```csharp
// Create sort options
var sortOptions = new SortOptions<Product>();
sortOptions.Add(p => p.Price, SortDirection.Descending);
sortOptions.Add(p => p.Name);  // Ascending is default

// Apply sorting
var products = await _repository.Query(
    filter: null,
    sortOptions: sortOptions
);
```

### Pagination

```csharp
// Skip 20 items and take 10
var productsPage = await _repository.Query(
    filter: null,
    sortOptions: null,
    skip: 20,
    take: 10
);

// Get total count for pagination
var totalCount = await _repository.Count();
```

### Combined Querying

```csharp
// Filter, sort, and paginate
var queryOptions = new QueryOptions<Product>
{
    Filter = p => p.Category == "Electronics",
    Sort = new SortOptions<Product>(),
    Skip = 10,
    Take = 5
};

queryOptions.Sort.Add(p => p.Price, SortDirection.Descending);

var products = await _repository.Query(queryOptions);
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
