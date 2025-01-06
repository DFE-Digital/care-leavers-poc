using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ConfluencePOC.Web.Extensions;

public static class DistributedCacheExtensions
{
    public static JsonSerializerSettings? SerializerSettings { get; set; }

    public static DistributedCacheEntryOptions DefaultCacheOptions { get; set; } =
        new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

    public static JsonSerializer? Serializer { get; set; }


    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
    {
        return SetAsync(cache, key, value, DefaultCacheOptions);
    }

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, SerializerSettings));
        return cache.SetAsync(key, bytes, options);
    }

    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        var val = cache.Get(key);
        value = default;
        if (val == null) return false;
        
        
        using (var stream = new MemoryStream(val))
        {
            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {

                value = Serializer.Deserialize<T>(reader);
            }
        }
        
        return true;
    }

    public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task, DistributedCacheEntryOptions? options = null, bool bypassCache = false)
    {
        if (options == null)
        {
            options = DefaultCacheOptions;
        }
        if (!bypassCache && cache.TryGetValue(key, out T? value) && value is not null)
        {
            return value;
        }
        value = await task();
        if (value is not null)
        {
            await cache.SetAsync<T>(key, value, options);
        }
        return value;
    }
}