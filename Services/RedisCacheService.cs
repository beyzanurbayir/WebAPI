using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IDatabase _redisDb;

    public RedisCacheService(IConnectionMultiplexer redisConnection)
    {
        _redisConnection = redisConnection;
        _redisDb = _redisConnection.GetDatabase();
    }

    public T Get<T>(string cacheKey)
    {
        var cachedData = _redisDb.StringGet(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        return default(T);
    }

    public void Set<T>(string cacheKey, T data, TimeSpan? expiry = null)
    {
        var jsonData = JsonSerializer.Serialize(data);
        _redisDb.StringSet(cacheKey, jsonData, expiry);
    }

    public void Remove(string cacheKey)
    {
        _redisDb.KeyDelete(cacheKey);
    }
}
