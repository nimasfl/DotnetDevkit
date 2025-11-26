using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDevkit.Cache.Test
{
    public class RedisCacheConnectionServiceTests
    {
        [Fact]
        public async Task StartAsync_ShouldStartBackgroundLoop_WhenCalled()
        {
            var options = Options.Create(new SafeRedisCacheOptions
            {
                HealthCheckIntervalSec = 1,
                RetryBaseDelayMs = 10,
                RetryMaxDelayMs = 50
            });

            var redisLogger = new FakeLogger<RedisCache>();

            var cache = new RedisCache(options, connectionFactory: null, redisLogger);

            var svcLogger = new FakeLogger<RedisCacheConnectionService>();
            var svc = new RedisCacheConnectionService(cache, options, svcLogger);

            await svc.StartAsync(CancellationToken.None);

            // give the background loop a short time to run and ensure no exceptions thrown
            await Task.Delay(100);

            await svc.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task StopAsync_ShouldCancelBackgroundLoop_WhenCalled()
        {
            var options = Options.Create(new SafeRedisCacheOptions
            {
                HealthCheckIntervalSec = 1,
                RetryBaseDelayMs = 10,
                RetryMaxDelayMs = 50
            });

            var redisLogger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, redisLogger);

            var svcLogger = new FakeLogger<RedisCacheConnectionService>();
            var svc = new RedisCacheConnectionService(cache, options, svcLogger);

            await svc.StartAsync(CancellationToken.None);
            await svc.StopAsync(CancellationToken.None);

            // ensure calling StopAsync again does not throw
            var ex = await Record.ExceptionAsync(() => svc.StopAsync(CancellationToken.None));
            Assert.Null(ex);
        }
    }
}
