using Microsoft.Extensions.Options;

namespace DotnetDevkit.Cache.Test
{
    public class RedisCacheTests
    {
        [Fact]
        public async Task EnsureConnectedAsync_ShouldReturnWithoutThrow_WhenFactoryIsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.EnsureConnectedAsync(CancellationToken.None));
            Assert.Null(ex);
            Assert.False(cache.IsConnected);
        }

        [Fact]
        public async Task EnsureConnectedAsync_ShouldNotSetConnection_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);

            var cache = new RedisCache(options, factory, logger);

            await cache.EnsureConnectedAsync();
            Assert.False(cache.IsConnected);
        }

        [Fact]
        public async Task EnsureConnectedAsync_ShouldNotThrow_WhenFactoryThrowsException()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromException<StackExchange.Redis.ConnectionMultiplexer?>(new InvalidOperationException("Connection failed"));

            var cache = new RedisCache(options, factory, logger);

            var ex = await Record.ExceptionAsync(() => cache.EnsureConnectedAsync());
            Assert.Null(ex);
            Assert.False(cache.IsConnected);
        }

        [Fact]
        public async Task EnsureConnectedAsync_ShouldRethrow_WhenFactoryThrowsOperationCanceledException()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromException<StackExchange.Redis.ConnectionMultiplexer?>(new OperationCanceledException());
            var cache = new RedisCache(options, factory, logger);

            await Assert.ThrowsAsync<OperationCanceledException>(() => cache.EnsureConnectedAsync());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNone_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetAsync<int>("missing-key", CancellationToken.None);
            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNone_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);
            var cache = new RedisCache(options, factory, logger);

            var result = await cache.GetAsync<string>("any-key");
            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync("k", 123, null, CancellationToken.None));
            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);
            var cache = new RedisCache(options, factory, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync("key", "value", TimeSpan.FromMinutes(1)));
            Assert.Null(ex);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.RemoveAsync("any-key"));
            Assert.Null(ex);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);
            var cache = new RedisCache(options, factory, logger);

            var ex = await Record.ExceptionAsync(() => cache.RemoveAsync("key-to-remove"));
            Assert.Null(ex);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldCallFactory_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var factoryCalled = false;
            var result = await cache.GetOrAddAsync("key", ct =>
            {
                factoryCalled = true;
                return Task.FromResult(42);
            });

            Assert.True(factoryCalled);
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldCallFactory_WhenFactoryReturnsNull()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var factory = () => Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);
            var cache = new RedisCache(options, factory, logger);

            var factoryCalled = false;
            var result = await cache.GetOrAddAsync("key", ct =>
            {
                factoryCalled = true;
                return Task.FromResult("generated-value");
            });

            Assert.True(factoryCalled);
            Assert.Equal("generated-value", result);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldReturnFactoryResult_WithCustomExpiry()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetOrAddAsync(
                "key",
                ct => Task.FromResult(100),
                absoluteExpiry: TimeSpan.FromHours(1));

            Assert.Equal(100, result);
        }

        [Fact]
        public void IsConnected_ShouldReturnFalse_WhenNotInitialized()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            Assert.False(cache.IsConnected);
        }

        [Fact]
        public void Dispose_ShouldNotThrow_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = Record.Exception(() => cache.Dispose());
            Assert.Null(ex);
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = Record.Exception(() =>
            {
                cache.Dispose();
                cache.Dispose();
            });
            Assert.Null(ex);
        }

        [Fact]
        public async Task EnsureConnectedAsync_ShouldBeThreadSafe_WhenCalledConcurrently()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var callCount = 0;
            var factory = () =>
            {
                Interlocked.Increment(ref callCount);
                return Task.FromResult<StackExchange.Redis.ConnectionMultiplexer?>(null);
            };
            var cache = new RedisCache(options, factory, logger);

            var tasks = Enumerable.Range(0, 10)
                .Select(_ => cache.EnsureConnectedAsync())
                .ToArray();

            await Task.WhenAll(tasks);

            // Factory should be called multiple times due to null result, but no exceptions
            Assert.True(callCount >= 1);
        }

        #region GetAsync Additional Tests

        [Fact]
        public async Task GetAsync_ShouldReturnNone_ForDifferentValueTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var intResult = await cache.GetAsync<int>("key");
            var doubleResult = await cache.GetAsync<double>("key");
            var boolResult = await cache.GetAsync<bool>("key");
            var guidResult = await cache.GetAsync<Guid>("key");

            Assert.False(intResult.HasValue);
            Assert.False(doubleResult.HasValue);
            Assert.False(boolResult.HasValue);
            Assert.False(guidResult.HasValue);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNone_ForReferenceTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var stringResult = await cache.GetAsync<string>("key");
            var arrayResult = await cache.GetAsync<int[]>("key");
            var listResult = await cache.GetAsync<List<string>>("key");

            Assert.False(stringResult.HasValue);
            Assert.False(arrayResult.HasValue);
            Assert.False(listResult.HasValue);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNone_ForComplexTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetAsync<TestDataObject>("complex-key");

            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task GetAsync_ShouldHandleEmptyKey()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetAsync<string>("");

            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task GetAsync_ShouldHandleSpecialCharactersInKey()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetAsync<string>("key:with:colons:and/slashes");

            Assert.False(result.HasValue);
        }

        #endregion

        #region SetAsync Additional Tests

        [Fact]
        public async Task SetAsync_ShouldNotThrow_ForDifferentValueTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex1 = await Record.ExceptionAsync(() => cache.SetAsync("int-key", 42));
            var ex2 = await Record.ExceptionAsync(() => cache.SetAsync("double-key", 3.14));
            var ex3 = await Record.ExceptionAsync(() => cache.SetAsync("bool-key", true));
            var ex4 = await Record.ExceptionAsync(() => cache.SetAsync("guid-key", Guid.NewGuid()));

            Assert.Null(ex1);
            Assert.Null(ex2);
            Assert.Null(ex3);
            Assert.Null(ex4);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_ForComplexTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var complexObject = new TestDataObject { Id = 1, Name = "Test", Values = [1, 2, 3] };
            var ex = await Record.ExceptionAsync(() => cache.SetAsync("complex-key", complexObject));

            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_ForNullValue()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync<string?>("key", null));

            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_WithZeroExpiry()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync("key", "value", TimeSpan.Zero));

            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldNotThrow_WithVeryLongExpiry()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.SetAsync("key", "value", TimeSpan.FromDays(365)));

            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldUseDefaultExpiry_WhenNotSpecified()
        {
            var options = Options.Create(new SafeRedisCacheOptions { DefaultExpiryMs = 60000 });
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            // Should not throw and use default expiry
            var ex = await Record.ExceptionAsync(() => cache.SetAsync("key", "value"));

            Assert.Null(ex);
        }

        [Fact]
        public async Task SetAsync_ShouldHandleCollectionTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var list = new List<int> { 1, 2, 3, 4, 5 };
            var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var array = new[] { "one", "two", "three" };

            var ex1 = await Record.ExceptionAsync(() => cache.SetAsync("list-key", list));
            var ex2 = await Record.ExceptionAsync(() => cache.SetAsync("dict-key", dict));
            var ex3 = await Record.ExceptionAsync(() => cache.SetAsync("array-key", array));

            Assert.Null(ex1);
            Assert.Null(ex2);
            Assert.Null(ex3);
        }

        #endregion

        #region RemoveAsync Additional Tests

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow_ForEmptyKey()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.RemoveAsync(""));

            Assert.Null(ex);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow_ForSpecialCharactersInKey()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(() => cache.RemoveAsync("key:with:colons:and/slashes"));

            Assert.Null(ex);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow_WhenCalledMultipleTimes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var ex = await Record.ExceptionAsync(async () =>
            {
                await cache.RemoveAsync("same-key");
                await cache.RemoveAsync("same-key");
                await cache.RemoveAsync("same-key");
            });

            Assert.Null(ex);
        }

        #endregion

        #region GetOrAddAsync Additional Tests

        [Fact]
        public async Task GetOrAddAsync_ShouldReturnFactoryResult_ForValueTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var intResult = await cache.GetOrAddAsync("int-key", _ => Task.FromResult(42));
            var doubleResult = await cache.GetOrAddAsync("double-key", _ => Task.FromResult(3.14));
            var boolResult = await cache.GetOrAddAsync("bool-key", _ => Task.FromResult(true));

            Assert.Equal(42, intResult);
            Assert.Equal(3.14, doubleResult);
            Assert.True(boolResult);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldReturnFactoryResult_ForComplexTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var expected = new TestDataObject { Id = 123, Name = "Test Object", Values = [10, 20, 30] };
            var result = await cache.GetOrAddAsync("complex-key", _ => Task.FromResult(expected));

            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Values, result.Values);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldPassCancellationToken_ToFactory()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            using var cts = new CancellationTokenSource();
            CancellationToken receivedToken = default;

            await cache.GetOrAddAsync("key", ct =>
            {
                receivedToken = ct;
                return Task.FromResult("value");
            }, cancellationToken: cts.Token);

            Assert.Equal(cts.Token, receivedToken);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldPropagateFactoryException()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                cache.GetOrAddAsync<string>("key", _ => throw new InvalidOperationException("Factory failed")));
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldPropagateOperationCanceledException_FromFactory()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                cache.GetOrAddAsync<string>("key", _ => throw new OperationCanceledException()));
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldCallFactoryOnce_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var callCount = 0;
            await cache.GetOrAddAsync("key", _ =>
            {
                Interlocked.Increment(ref callCount);
                return Task.FromResult("value");
            });

            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldHandleAsyncFactory()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetOrAddAsync("key", async ct =>
            {
                await Task.Delay(10, ct);
                return "async-value";
            });

            Assert.Equal("async-value", result);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldReturnDefaultForValueType_WhenFactoryReturnsDefault()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetOrAddAsync("key", _ => Task.FromResult(0));

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldHandleNullFactoryResult_ForReferenceTypes()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result = await cache.GetOrAddAsync<string?>("key", _ => Task.FromResult<string?>(null));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldWorkWithDifferentExpiryValues()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var result1 = await cache.GetOrAddAsync("key1", _ => Task.FromResult(1), TimeSpan.FromSeconds(1));
            var result2 = await cache.GetOrAddAsync("key2", _ => Task.FromResult(2), TimeSpan.FromMinutes(30));
            var result3 = await cache.GetOrAddAsync("key3", _ => Task.FromResult(3), TimeSpan.FromHours(24));

            Assert.Equal(1, result1);
            Assert.Equal(2, result2);
            Assert.Equal(3, result3);
        }

        [Fact]
        public async Task GetOrAddAsync_ShouldHandleConcurrentCalls_WhenNotConnected()
        {
            var options = Options.Create(new SafeRedisCacheOptions());
            var logger = new FakeLogger<RedisCache>();
            var cache = new RedisCache(options, connectionFactory: null, logger);

            var callCount = 0;
            var tasks = Enumerable.Range(0, 10).Select(_ =>
                cache.GetOrAddAsync("same-key", ct =>
                {
                    Interlocked.Increment(ref callCount);
                    return Task.FromResult("value");
                })).ToArray();

            var results = await Task.WhenAll(tasks);

            // All results should be the same
            Assert.All(results, r => Assert.Equal("value", r));
            // Factory called for each concurrent call since not connected (no cache coordination)
            Assert.Equal(10, callCount);
        }

        #endregion
    }

    public class TestDataObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> Values { get; set; } = [];
    }
}
