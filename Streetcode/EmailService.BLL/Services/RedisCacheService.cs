using EmailService.BLL.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        // == create
        public async Task SetAsync(string key, string value, TimeSpan expiration)
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value, expiration);
        }

        // == select
        public async Task<string> GetAsync(string key)
        {
            var db = _redisConnection.GetDatabase();
            return await db.StringGetAsync(key); 
        }

        // == delete
        public async Task RemoveAsync(string key)
        {
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
