namespace DotnetDevkit.Cache.Test
    {
        public class SafeRedisCacheOptionsTests
        {
            [Fact]
            public void Properties_ShouldHaveDefaults_WhenInstantiated()
            {
                var opts = new SafeRedisCacheOptions();

                Assert.True(opts.DefaultExpiryMs > 0);
                Assert.True(opts.LockExpiryMs > 0);
                Assert.True(opts.LockWaitMs > 0);
                Assert.True(opts.HealthCheckIntervalSec > 0);
                Assert.True(opts.RetryBaseDelayMs > 0);
                Assert.True(opts.RetryMaxDelayMs > 0);
            }
        }
    }
