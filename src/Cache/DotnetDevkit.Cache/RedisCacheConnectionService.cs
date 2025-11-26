using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDevkit.Cache;

public class RedisCacheConnectionService(
    RedisCache cache,
    IOptions<SafeRedisCacheOptions> options,
    ILogger<RedisCacheConnectionService> logger)
    : IHostedService, IDisposable
{
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task.Run(() => BackgroundLoopAsync(cancellationToken), CancellationToken.None);
        return Task.CompletedTask;
    }

    private async Task BackgroundLoopAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        var rnd = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await cache.EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

                if (cache.IsConnected)
                {
                    attempt = 0;
                    var delaySeconds = Math.Max(1, options.Value.HealthCheckIntervalSec);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    attempt++;
                    var backoffMs = CalculateBackoffMs(attempt, options.Value.RetryBaseDelayMs,
                        options.Value.RetryMaxDelayMs);
                    // add jitter
                    backoffMs += rnd.Next(0, Math.Max(1, options.Value.RetryBaseDelayMs));
                    logger.LogInformation("Redis not connected; retrying in {BackoffMs}ms (attempt {Attempt}).",
                        backoffMs, attempt);
                    await Task.Delay(backoffMs, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unhandled exception in Redis connection background loop.");
                await Task.Delay(TimeSpan.FromMilliseconds(options.Value.RetryBaseDelayMs), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private static int CalculateBackoffMs(int attempt, int baseMs, int maxMs)
    {
        if (attempt <= 0) return baseMs;
        var exponent = Math.Min(attempt, 10);
        var ms = Math.Min(maxMs, baseMs * (1 << (exponent - 1)));
        return ms;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
