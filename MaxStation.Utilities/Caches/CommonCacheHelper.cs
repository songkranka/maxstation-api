using MaxStation.Utilities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MaxStation.Utilities.Caches;

public class CommonCacheHelper : ICommonCacheHelper
{
    private readonly IMemoryCache _cache;
    private readonly CacheOption _cacheOption;

    public CommonCacheHelper(IMemoryCache cache, IOptions<CacheOption> cacheOption)
    {
        _cache = cache;
        _cacheOption = cacheOption.Value;
    }

    public string GetRequestName(string key)
        => $"{key}_request";

    public string GetResponseName(string key)
        => $"{key}_response";

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");

        return await Task.FromResult(_cache.Get<T>(key));
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        if (!_cache.Get(key).IsNullable())
        {
            _cache.Remove(key);
        }
    }

    /// <exception cref="ArgumentException"></exception>
    /// <inheritdoc/>
    public Task CreateAsync<T>(string key, T item) where T : class
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
        if (item == null) throw new ArgumentException("item");

        var option = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(_cacheOption.SlidingExpiration),
            AbsoluteExpiration = DateTime.UtcNow.AddDays(_cacheOption.AbSoluteExpire),
            Priority = CacheItemPriority.Normal
        };

        _cache.Set(key, item, option);

        return Task.CompletedTask;
    }

    /// <exception cref="ArgumentException"></exception>
    /// <inheritdoc/>
    public Task CreateAsync<TSource, TResult>(string requestKey, TSource requestItem, string responseKey, TResult resultItem) where TSource : class where TResult : class
    {
        if (string.IsNullOrWhiteSpace(requestKey)) throw new ArgumentException("requestKey");
        if (string.IsNullOrWhiteSpace(responseKey)) throw new ArgumentException("responseKey");

        if (requestItem == null) throw new ArgumentException("requestItem");
        if (resultItem == null) throw new ArgumentException("resultItem");

        var option = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(_cacheOption.SlidingExpiration),
            AbsoluteExpiration = DateTime.UtcNow.AddDays(_cacheOption.AbSoluteExpire)
        };

        _cache.Set(requestKey, requestItem, option);
        _cache.Set(responseKey, resultItem, option);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task CreateAsync<TSource, TResult>(string key, TSource requestItem, TResult resultItem) where TSource : class where TResult : class
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
        if (requestItem == null) throw new ArgumentException("requestItem");
        if (resultItem == null) throw new ArgumentException("resultItem");

        var option = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(_cacheOption.SlidingExpiration),
            AbsoluteExpiration = DateTime.UtcNow.AddDays(_cacheOption.AbSoluteExpire)
        };

        _cache.Set(GetRequestName(key), requestItem, option);
        _cache.Set(GetResponseName(key), resultItem, option);

        return Task.CompletedTask;
    }
}