using FluentCMS.Repositories.Abstractions.Repositories;
using FluentCMS.Repositories.Sqlite.Configuration;
using FluentCMS.Repositories.Sqlite.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentCMS.Repositories.Sqlite.Extensions;

/// <summary>
/// Extension methods for setting up SQLite repositories in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQLite repositories to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The configuration containing SQLite settings.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqliteRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SqliteOptions>(options => 
            configuration.GetSection(nameof(SqliteOptions)).Bind(options));

        services.AddScoped<FluentCmsDbContext>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqliteOptions>>().Value;
            return new FluentCmsDbContext(options);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(SqliteRepository<>));

        return services;
    }

    /// <summary>
    /// Adds SQLite repositories to the specified <see cref="IServiceCollection" /> with custom configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configureOptions">The action used to configure the SQLite options.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqliteRepositories(
        this IServiceCollection services,
        Action<SqliteOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddScoped<FluentCmsDbContext>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqliteOptions>>().Value;
            return new FluentCmsDbContext(options);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(SqliteRepository<>));

        return services;
    }
}
