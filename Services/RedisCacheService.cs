using StackExchange.Redis;
using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;  // ILogger için gerekli

public class RedisCacheService
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IDatabase _redisDb;
    private readonly ILogger<RedisCacheService> _logger;  // Logger ekleyin

    public RedisCacheService(IConnectionMultiplexer redisConnection, ILogger<RedisCacheService> logger)
    {
        _redisConnection = redisConnection;
        _redisDb = _redisConnection.GetDatabase();
        _logger = logger;  // Logger'ı yapıcıya ekleyin
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

    public bool Remove(string key)
    {
        try
        {
            return _redisDb.KeyDelete(key);  // Silme işlemi
        }
        catch (Exception ex)
        {
            _logger.LogError($"Redis'ten anahtar silinirken hata oluştu: {ex.Message}");
            return false;
        }
    }
}
