# Technical Context

## Development Environment

### .NET Version
- Target Framework: .NET 8.0
- Language Version: C# 12.0

### Development Tools
- Visual Studio 2022 or later
- Visual Studio Code with C# extensions
- JetBrains Rider

### Source Control
- Git
- GitHub for repository hosting

## Dependencies

### Core Framework Dependencies
- **Microsoft.Extensions.DependencyInjection**: For dependency injection support
- **Microsoft.Extensions.Options**: For options pattern implementation

### MongoDB Implementation
- **MongoDB.Driver**: Official MongoDB driver for .NET

### SQL Server Implementation
- **Microsoft.EntityFrameworkCore.SqlServer**: EF Core provider for SQL Server

### SQLite Implementation
- **Microsoft.EntityFrameworkCore.Sqlite**: EF Core provider for SQLite

### Testing Dependencies
- **xUnit**: Testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Fluent assertion library for more readable tests

## Project Structure

```
FluentCMS.Repositories/
├── src/
│   ├── FluentCMS.Repositories.Abstractions/
│   │   ├── Entities/
│   │   │   └── IBaseEntity.cs
│   │   ├── Exceptions/
│   │   │   └── RepositoryExceptions.cs
│   │   ├── Models/
│   │   │   └── QueryOptions.cs
│   │   └── Repositories/
│   │       └── IBaseEntityRepository.cs
│   ├── FluentCMS.Repositories.MongoDb/
│   │   ├── Configuration/
│   │   │   └── MongoDbOptions.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Repositories/
│   │       └── MongoDbRepository.cs
│   ├── FluentCMS.Repositories.SqlServer/
│   │   ├── Configuration/
│   │   │   └── SqlServerOptions.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Repositories/
│   │       └── SqlServerRepository.cs
│   └── FluentCMS.Repositories.Sqlite/
│       ├── Configuration/
│       │   └── SqliteOptions.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Repositories/
│           └── SqliteRepository.cs
├── tests/
│   ├── FluentCMS.Repositories.MongoDb.Tests/
│   ├── FluentCMS.Repositories.SqlServer.Tests/
│   └── FluentCMS.Repositories.Sqlite.Tests/
└── samples/
    └── FluentCMS.Repositories.Samples/
```

## Key Technical Considerations

### Entity Framework Core Usage

For SQL Server and SQLite implementations:
- Using EF Core's DbContext and DbSet
- LINQ provider for query translation
- Change tracking for updates
- No direct SQL queries to maintain database agnosticism

### MongoDB Driver Usage

For MongoDB implementation:
- Using the official MongoDB C# driver
- LINQ provider support for query capabilities
- BsonClassMap for entity mapping
- Aggregation pipeline for complex queries

### Asynchronous Operations

- All repository operations must be fully asynchronous
- Use of Task-based Asynchronous Pattern (TAP)
- No mixing of synchronous and asynchronous code

### Query Optimization

- Efficient index usage
- Projection for performance when fetching large datasets
- Careful handling of tracking vs. no-tracking queries in EF Core
- Proper use of MongoDB's projection and aggregation pipeline

### Error Handling

- Custom exceptions for repository-specific errors
- Consistent error handling across all implementations
- Meaningful error messages that don't expose database implementation details

### Performance Monitoring

- Performance tracking using custom diagnostic listeners
- Repository operation timings and query logging
- Optionally integrates with ApplicationInsights or other monitoring tools

## Database Schema Considerations

The library is designed to work with existing schemas, but some conventions are recommended:

### MongoDB
- Collections named after entity types (pluralized)
- Consistent use of MongoDB's ObjectId type mapped to GUIDs

### SQL Server and SQLite
- Tables named after entity types (pluralized)
- Primary key column named 'Id' of type UNIQUEIDENTIFIER

## Development Workflows

### Build Process
```bash
dotnet restore
dotnet build
```

### Testing Workflow
```bash
dotnet test
```

### Local Development Setup
- Local MongoDB instance for testing (can be dockerized)
- LocalDB or SQL Express for SQL Server testing
- In-memory SQLite for SQLite testing

## Deployment Considerations

### Versioning
- Semantic versioning
- Breaking changes only in major versions

### Package Distribution
- NuGet packages for each implementation
- Metapackage for all implementations

### Documentation
- XML documentation for public APIs
- README files with usage examples
- Sample projects demonstrating different scenarios
