namespace FluentCMS.Repositories.SqlServer.Configuration;

/// <summary>
/// Configuration options for SQL Server connections.
/// </summary>
public class SqlServerOptions
{
    /// <summary>
    /// Gets or sets the SQL Server connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "Server=(localdb)\\mssqllocaldb;Database=FluentCMS;Trusted_Connection=True;";
}
