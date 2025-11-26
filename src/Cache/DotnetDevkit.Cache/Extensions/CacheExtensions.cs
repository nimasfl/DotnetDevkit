using DotnetDevkit.Cache.Abstractions;
using DotnetDevkit.Cache.Models;
using DotnetDevkit.Cache.Strategies;
using DotnetDevkit.Cache.Strategies.Memory;
using DotnetDevkit.Cache.Strategies.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevkit.Cache.Extensions;

public static class CacheExtensions
{
    extension(IServiceCollection services)
    {
        public void AddCache(CacheProviderOptions options)
        {
            services.RegisterServices(options);
        }
        public void AddCache(IConfiguration configuration, string configSectionPath)
        {
            services.AddOptions<CacheProviderOptions>()
                .BindConfiguration(configSectionPath)
                .Validate(CacheProviderOptions.Validate)
                .ValidateOnStart();

            var options = configuration.GetValue<CacheProviderOptions>(configSectionPath)!;

            services.RegisterServices(options);
        }

        private void RegisterServices(CacheProviderOptions options)
        {
            switch (options.Strategy)
            {
                case CacheStrategy.Redis:
                    services.AddScoped<RedisCacheStrategy>();
                    services.AddScoped<ICacheStrategy, RedisExceptionHandlerDecorator>();
                    break;

                case CacheStrategy.Memory:
                    services.AddScoped<ICacheStrategy, MemoryCacheStrategy>();
                    break;

                case CacheStrategy.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            services.AddScoped<ICacheProvider, CacheProvider>();
        }
    }
}
