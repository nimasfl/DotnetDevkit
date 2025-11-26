// csharp

using DotnetDevkit.Cache.Abstractions;

namespace DotnetDevkit.Cache.Test
    {
        public class CacheResultTests
        {
            [Fact]
            public void Some_ShouldHaveValue_WhenCalled()
            {
                var r = CacheResult<int>.Some(42);
                Assert.True(r.HasValue);
                Assert.Equal(42, r.Value);
            }

            [Fact]
            public void None_ShouldNotHaveValue_WhenAccessed()
            {
                var r = CacheResult<int>.None;
                Assert.False(r.HasValue);
            }
        }
    }
