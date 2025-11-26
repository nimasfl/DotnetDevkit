using DotnetDevkit.Cache.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetDevkit.Cache.Test
{
    public class ExtensionsTests
    {
        [Fact]
        public void AddCache_ShouldRegisterServices_WhenCalled()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            var config = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddCache("RedisSection");

            using var sp = services.BuildServiceProvider();

            var redisCache = sp.GetService<RedisCache>();
            Assert.NotNull(redisCache);

            // ICache should resolve to the registered RedisCache instance
            var cache = sp.GetService<ICache>();
            Assert.NotNull(cache);

            // Connection factory should be registered
            var factory =
                sp.GetService<Func<Task<StackExchange.Redis.ConnectionMultiplexer?>>>();
            Assert.NotNull(factory);

            // HostedService should include RedisCacheConnectionService
            var hosted = sp.GetServices<IHostedService>();
            Assert.Contains(hosted, h => h.GetType() == typeof(RedisCacheConnectionService));
        }
    }
}
