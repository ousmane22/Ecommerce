using Ecommerce.Common.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre un repository générique
    /// </summary>
    public static IServiceCollection AddRepository<TRepository, TInterface>(
        this IServiceCollection services)
        where TRepository : class, TInterface
        where TInterface : class
    {
        services.AddScoped<TInterface, TRepository>();
        return services;
    }

    /// <summary>
    /// Configure une section de configuration de manière générique
    /// </summary>
    public static IServiceCollection ConfigureSettings<TSettings>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TSettings : class
    {
        services.Configure<TSettings>(configuration.GetSection(sectionName));
        return services;
    }
}

