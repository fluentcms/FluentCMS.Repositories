namespace FluentCMS.Repositories.Sqlite.Configuration;

/// <summary>
/// Configuration options for SQLite connections.
/// </summary>
public class SqliteOptions
{
    /// <summary>
    /// Gets or sets the SQLite connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "Data Source=FluentCMS.db";
}
