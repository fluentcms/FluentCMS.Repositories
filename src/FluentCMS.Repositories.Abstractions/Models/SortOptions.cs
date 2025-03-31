using System.Linq.Expressions;

namespace FluentCMS.Repositories.Abstractions.Models;

/// <summary>
/// Specifies the direction for sorting operations.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    Ascending,
    
    /// <summary>
    /// Sort in descending order.
    /// </summary>
    Descending
}

/// <summary>
/// Represents a single sort expression with a direction.
/// </summary>
/// <typeparam name="T">The type of entity being sorted.</typeparam>
public class SortExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortExpression{T}"/> class.
    /// </summary>
    /// <param name="expression">The expression used for sorting.</param>
    /// <param name="direction">The direction of sorting.</param>
    public SortExpression(LambdaExpression expression, SortDirection direction)
    {
        Expression = expression;
        Direction = direction;
    }

    /// <summary>
    /// Gets the expression used for sorting.
    /// </summary>
    public LambdaExpression Expression { get; }
    
    /// <summary>
    /// Gets the direction of sorting.
    /// </summary>
    public SortDirection Direction { get; }
}

/// <summary>
/// Provides options for sorting entities in queries.
/// </summary>
/// <typeparam name="T">The type of entity being sorted.</typeparam>
public class SortOptions<T>
{
    /// <summary>
    /// Gets the list of sort expressions.
    /// </summary>
    public List<SortExpression<T>> Expressions { get; } = new();

    /// <summary>
    /// Adds a sort expression to the collection.
    /// </summary>
    /// <typeparam name="TKey">The type of the property being sorted on.</typeparam>
    /// <param name="expression">The expression used for sorting.</param>
    /// <param name="direction">The direction of sorting.</param>
    public void Add<TKey>(Expression<Func<T, TKey>> expression, SortDirection direction = SortDirection.Ascending)
    {
        Expressions.Add(new SortExpression<T>(expression, direction));
    }
}
