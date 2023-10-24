namespace MaxStation.Utilities.Caches;

/// <summary>
/// Cache option settings
/// </summary>
[Serializable]
public class CacheOption
{
    /// <summary>
    /// Sliding Expiration (Seconds)
    /// </summary>
    public int SlidingExpiration { get; set; }

    /// <summary>
    /// Absolute Expire(Days)
    /// </summary>
    public int AbSoluteExpire { get; set; }
}