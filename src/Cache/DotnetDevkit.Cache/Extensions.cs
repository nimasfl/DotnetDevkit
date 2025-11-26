using DotnetDevkit.Cache.Abstractions;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DotnetDevkit.Cache;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCache(string configurationSectionPath)
        {
            services.AddOptions<SafeRedisCacheOptions>()
                .BindConfiguration(configurationSectionPath)
                .Validate(o => o != null && (o.Configuration != null || o.ConfigurationOptions != null))
                .ValidateOnStart();

            services.AddSingleton<Func<Task<ConnectionMultiplexer?>>>(sp =>
            {
                return async () =>
                {
                    var options = sp.GetRequiredService<IOptions<SafeRedisCacheOptions>>().Value;

                    try
                    {
                        if (options.Configuration is not null)
                        {
                            return await ConnectionMultiplexer.ConnectAsync(options.Configuration)
                                .ConfigureAwait(false);
                        }

                        if (options.ConfigurationOptions is not null)
                        {
                            return await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions)
                                .ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignore
                    }

                    return null;
                };
            });

            // Provide RedisCache with IOptions<RedisCacheOptions> and the connection factory
            services.AddScoped<RedisCache>();

            services.AddScoped<ICache, RedisCacheReconnector>();

            return services;
        }
    }
}
