using Newtonsoft.Json;

namespace DotnetDevkit.Cache.Abstractions;

public record CacheContext
{
    public DateTime? AbsoluteExpiration { get; private set; }
    public TimeSpan? SlidingTimespan { get; private set; }

    internal CacheContext()
    {
    }

    [JsonConstructor]
    internal CacheContext(DateTime? absoluteExpiration, TimeSpan? slidingTimespan)
    {
        AbsoluteExpiration = absoluteExpiration;
        SlidingTimespan = slidingTimespan;
    }

    public static CacheContext Create()
    {
        return new CacheContext();
    }

    public void SetExpiration(DateTime dateTime)
    {
        AbsoluteExpiration = dateTime;
    }
    public void SetExpiration(DateTime dateTime, TimeSpan slidingTimespan)
    {
        SetExpiration(dateTime);
        SlidingTimespan = slidingTimespan;
    }
    public void SetExpiration(TimeSpan timespan)
    {
        AbsoluteExpiration = DateTime.UtcNow + timespan;
    }
    public void SetExpiration(TimeSpan timespan, TimeSpan slidingTimespan)
    {
        SetExpiration(timespan);
        SlidingTimespan = slidingTimespan;
    }
    public void SetExpiration(TimeSpan timespan, bool enableSliding)
    {
        if (enableSliding)
        {
            SetExpiration(timespan, timespan);
        }

        SetExpiration(timespan);
    }


    internal void SlideExpirationDate()
    {
        if (SlidingTimespan is null)
        {
            return;
        }

        AbsoluteExpiration = DateTime.UtcNow + SlidingTimespan;
    }

    internal bool IsExpired()
    {
        return AbsoluteExpiration == null || AbsoluteExpiration < DateTime.UtcNow;
    }
}
