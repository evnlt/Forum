using System.Text.Json;
using Microsoft.AspNetCore.OutputCaching;
using StackExchange.Redis;

namespace Forum.WebApi.Cashing;

public class RedisOutputCacheStore : IOutputCacheStore
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisOutputCacheStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }
    
    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<byte[]?>(value);
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, JsonSerializer.Serialize(value), validFor);
    }

    public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}