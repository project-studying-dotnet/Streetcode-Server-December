using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Streetcode.BLL.Interfaces.RedisCache;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Streetcode.BLL.Services.RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetCachedDataAsync<T>(string key)
        {
            var cachedData = await _cache.GetAsync(key);
            if (cachedData == null)
            {
                return default;
            }

            var stringData = Encoding.UTF8.GetString(cachedData);
            return stringData == null ? default : JsonConvert.DeserializeObject<T>(stringData);
        }

        public async Task SetCachedDataAsync<T>(string key, T data, int minutes)
        {
            var serializedData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var cachedData = Encoding.UTF8.GetBytes(serializedData);

            await _cache.SetAsync(key, cachedData, new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(minutes)));
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
