using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.DAL.Caching.RedisCache
{
    public interface IRedisCacheService
    {
        Task<T?> GetCachedDataAsync<T>(string key);
        Task SetCachedDataAsync<T>(string key, T data, int minutes);
        public Task RemoveDataAsync(string key);
    }
}
