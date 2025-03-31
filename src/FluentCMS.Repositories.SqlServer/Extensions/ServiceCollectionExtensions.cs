using FluentCMS.Repositories.Abstractions.Repositories;
using FluentCMS.Repositories.SqlServer.Configuration;
using FluentCMS.Repositories.SqlServer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentCMS.Repositories.SqlServer.Extensions;

/// <summary>
/// Extension methods for setting up SQL Server repositories in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQL Server repositories to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The configuration containing SQL Server settings.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqlServerRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SqlServerOptions>(options => 
            configuration.GetSection(nameof(SqlServerOptions)).Bind(options));

        services.AddScoped<FluentCmsDbContext>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqlServerOptions>>().Value;
            return new FluentCmsDbContext(options);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(SqlServerRepository<>));

        return services;
    }

    /// <summary>
    /// Adds SQL Server repositories to the specified <see cref="IServiceCollection" /> with custom configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configureOptions">The action used to configure the SQL Server options.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqlServerRepositories(
        this IServiceCollection services,
        Action<SqlServerOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddScoped<FluentCmsDbContext>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqlServerOptions>>().Value;
            return new FluentCmsDbContext(options);
        });

        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(SqlServerRepository<>));

        return services;
    }
}
