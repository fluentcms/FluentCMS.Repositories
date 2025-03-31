namespace FluentCMS.Repositories.Abstractions.Exceptions;

/// <summary>
/// Base exception for repository-related errors.
/// </summary>
public class RepositoryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public RepositoryException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : RepositoryException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="id">The entity ID that was not found.</param>
    /// <param name="entityName">The name of the entity type.</param>
    public EntityNotFoundException(Guid id, string entityName) 
        : base($"Entity of type {entityName} with id {id} was not found.") { }
}

/// <summary>
/// Exception thrown when a repository operation fails.
/// </summary>
public class RepositoryOperationException : RepositoryException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryOperationException"/> class.
    /// </summary>
    /// <param name="operation">The name of the operation that failed.</param>
    /// <param name="message">The error message.</param>
    public RepositoryOperationException(string operation, string message) 
        : base($"Repository operation '{operation}' failed: {message}") { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryOperationException"/> class.
    /// </summary>
    /// <param name="operation">The name of the operation that failed.</param>
    /// <param name="innerException">The inner exception.</param>
    public RepositoryOperationException(string operation, Exception innerException) 
        : base($"Repository operation '{operation}' failed.", innerException) { }
}
