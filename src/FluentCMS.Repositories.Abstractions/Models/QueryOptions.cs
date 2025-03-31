using System.Linq.Expressions;

namespace FluentCMS.Repositories.Abstractions.Models;

public class QueryOptions<T>
{
    // Filter to apply to the query
    public Expression<Func<T, bool>>? Filter { get; set; }
    
    // Sorting options
    public SortOptions<T>? Sort { get; set; }
    
    // Pagination: number of items to skip
    public int? Skip { get; set; }
    
    // Pagination: number of items to take
    public int? Take { get; set; }
}
