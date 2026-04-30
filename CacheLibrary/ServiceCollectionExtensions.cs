using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CacheLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCacheLibrary(this IServiceCollection services, IConfiguration? configuration = null)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICacheKeyBuilder, CacheKeyBuilder>();
        services.TryAddSingleton<ICacheBypassPolicy, CacheBypassPolicy>();

        if (configuration is not null)
        {
            services.Configure<CachingOptions>(configuration.GetSection(CachingOptions.SectionName));
        }
        else
        {
            services.AddOptions<CachingOptions>().Configure<IConfiguration>((opts, config) =>
            {
                config.GetSection(CachingOptions.SectionName).Bind(opts);
            });
        }

        return services;
    }

    public static IServiceCollection AddCacheBypass(this IServiceCollection services, string @namespace, string producer)
    {
        services.Configure<CacheBypassOptions>(o =>
        {
            o.Namespace = @namespace;
            o.Producer = producer;
        });
        services.TryAddSingleton<BypassHeaderPolicy>();
        return services;
    }
}
