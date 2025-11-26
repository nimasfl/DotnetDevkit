using DotnetDevkit.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Cache;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCache(string configurationSectionPath)
        {
            services.AddOptions<RedisCacheOptions>()
                .BindConfiguration(configurationSectionPath)
                .Validate()
                .ValidateOnStart();

            services.AddScoped<RedisCache>();
            services.AddScoped<ICache, RedisCacheReconnector>();
        }
    }
}
