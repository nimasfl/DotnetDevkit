using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDevkit.Cache.Test
{
    public class RedisCacheTests
    {
        [Fact]
        public async Task EnsureConnectedAsync_ShouldReturnWithoutThrow_WhenFactoryIsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = A.Fake<ILogger<RedisCache>>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.EnsureConnectedAsync(CancellationToken.None));
            Assert.Null(ex);
            Assert.False(cache.IsConnected);
        }

        [Fact]
        public async Task EnsureConnectedAsync_ShouldNotSetConnection_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = A.Fake<ILogger<RedisCache>>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);

            var cache = new RedisCache(options, factory, logger);

            await cache.EnsureConnectedAsync();
            Assert.False(cache.IsConnected);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNone_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = A.Fake<ILogger<RedisCache>>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetAsync<int>("missing-key", CancellationToken.None);
            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = A.Fake<ILogger<RedisCache>>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync("k", 123, null, CancellationToken.None));
            Assert.Null(ex);
        }
    }
}
