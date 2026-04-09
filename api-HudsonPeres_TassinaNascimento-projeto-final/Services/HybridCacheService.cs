using Microsoft.Extensions.Caching.Memory;
using api_HudsonPeres_TassinaNascimento_projeto_final.Services;

namespace api_HudsonPeres_TassinaNascimento_projeto_final.Services;

public class HybridCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly RedisCacheService _redisCache;

    public HybridCacheService(IMemoryCache memoryCache, RedisCacheService redisCache)
    {
        _memoryCache = memoryCache;
        _redisCache = redisCache;
    }
    public async Task RemoveAsync(string key)
{
    _memoryCache.Remove(key);
    await _redisCache.RemoveAsync(key);
}

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? memoryExpiry = null, TimeSpan? redisExpiry = null)
    {
        // 1º nível: cache local (Polly in-memory)
        if (_memoryCache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
            return cachedValue;

        // 2º nível: Redis
        var redisValue = await _redisCache.GetAsync<T>(key);
        if (redisValue != null)
        {
            _memoryCache.Set(key, redisValue, memoryExpiry ?? TimeSpan.FromSeconds(30));
            return redisValue;
        }

        // 3º nível: origem (base de dados, etc.)
        var data = await factory();

        if (data != null)
        {
            await _redisCache.SetAsync(key, data, redisExpiry ?? TimeSpan.FromMinutes(5));
            _memoryCache.Set(key, data, memoryExpiry ?? TimeSpan.FromSeconds(30));
        }

        return data;
    }
}