namespace FluentCMS.Repositories.MongoDb.Configuration;

/// <summary>
/// Configuration options for MongoDB connections.
/// </summary>
public class MongoDbOptions
{
    /// <summary>
    /// Gets or sets the MongoDB connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    
    /// <summary>
    /// Gets or sets the MongoDB database name.
    /// </summary>
    public string DatabaseName { get; set; } = "FluentCMS";
}
