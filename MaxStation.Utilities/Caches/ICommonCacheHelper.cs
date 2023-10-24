namespace MaxStation.Utilities.Caches;

public interface ICommonCacheHelper
{
    /// <summary>
    /// Get reuqest name
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    string GetRequestName(string key);

    /// <summary>
    /// Get response name
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns></returns>
    string GetResponseName(string key);

    /// <summary>
    /// Create cache with async
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="key">Key of cache</param>
    /// <param name="item">Data for add into cache</param>
    /// <returns></returns>
    Task CreateAsync<T>(string key, T item) where T : class;

    /// <summary>
    /// Create cache with async
    /// </summary>
    /// <typeparam name="TSource">Source type of data</typeparam>
    /// <typeparam name="TResult">Return type of result</typeparam>
    /// <param name="requestKey">Request key of cache</param>
    /// <param name="requestItem">Source data for add into cache</param>
    /// <param name="responseKey">Response key of cache</param>
    /// <param name="resultItem">Result data for add into cache</param>
    /// <returns></returns>
    Task CreateAsync<TSource, TResult>(string requestKey, TSource requestItem, string responseKey, TResult resultItem)
        where TSource : class
        where TResult : class;

    /// <summary>
    /// Create cache with async
    /// </summary>
    /// <typeparam name="TSource">Source type of data</typeparam>
    /// <typeparam name="TResult">Return type of result</typeparam>
    /// <param name="key">Key of cache</param>
    /// <param name="requestItem">Source data for add into cache</param>
    /// <param name="resultItem">Result data for add into cache</param>
    /// <returns></returns>
    Task CreateAsync<TSource, TResult>(string key, TSource requestItem, TResult resultItem)
        where TSource : class
        where TResult : class;

    /// <summary>
    /// Get cache with async
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="key">Key of cache</param>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Remove cache.
    /// </summary>
    /// <param name="key">Key of cache</param>
    void Remove(string key);
}