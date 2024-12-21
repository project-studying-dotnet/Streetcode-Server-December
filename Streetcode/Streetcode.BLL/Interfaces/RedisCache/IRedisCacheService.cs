using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Interfaces.RedisCache
{
    public interface IRedisCacheService
    {
        Task<T?> GetCachedDataAsync<T>(string key);
        Task SetCachedDataAsync<T>(string key, T data, int minutes);
        public Task RemoveDataAsync(string key);
    }
}
