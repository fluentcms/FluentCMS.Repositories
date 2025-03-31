# FluentCMS.Repositories - Project Brief

## Project Overview

FluentCMS.Repositories is a C# library that provides a generic repository pattern implementation supporting multiple database types including MongoDB, SQL Server, and SQLite. The goal is to create an abstraction layer that reduces redundant CRUD operations for domain entities in applications.

## Core Requirements

### General Requirements

- Create a generic repository interface that can work with any entity type having a GUID identifier
- Support multiple database types (MongoDB, SQL Server, SQLite) with specific implementations
- Maintain a consistent API across all database implementations
- Focus on atomic, granular operations without transaction support
- Follow clean code principles and C# best practices

### Entity Requirements

- All entities must implement an interface with a GUID Id property
- Support for any entity type that satisfies this basic interface requirement

## Repository Operations

### Read Operations

- Get entity by ID
- Get all entities
- Filtered query with support for: custom filter expressions, multiple sorting criteria (ascending and descending), pagination (skip and take)
- Count entities with optional filtering

### Write Operations

- Add a single entity
- Add multiple entities
- Update an entity
- Delete an entity
- Delete multiple entities
