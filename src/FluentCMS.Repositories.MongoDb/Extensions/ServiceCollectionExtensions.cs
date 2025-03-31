using FluentCMS.Repositories.Abstractions.Repositories;
using FluentCMS.Repositories.MongoDb.Configuration;
using FluentCMS.Repositories.MongoDb.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FluentCMS.Repositories.MongoDb.Extensions;

/// <summary>
/// Extension methods for setting up MongoDB repositories in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MongoDB repositories to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The configuration containing MongoDB settings.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMongoDbRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(options => 
            configuration.GetSection(nameof(MongoDbOptions)).Bind(options));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(MongoDbRepository<>));

        return services;
    }

    /// <summary>
    /// Adds MongoDB repositories to the specified <see cref="IServiceCollection" /> with custom configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configureOptions">The action used to configure the MongoDB options.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMongoDbRepositories(
        this IServiceCollection services,
        Action<MongoDbOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(MongoDbRepository<>));

        return services;
    }
}
