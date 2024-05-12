using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.DataAccess.Redis.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DisposableDomainDetector.Core.DataAccess.Redis.Implementations
{
    public class RedisRepository : IRedisRepository
    {
        private readonly int _expiryInMinutes;
        private readonly ILogger _logger;
        private IDatabase RedisDatabase => _redisMultiplexer.GetDatabase(0);
        private readonly IConnectionMultiplexer _redisMultiplexer;

        public RedisRepository(
            ILogger<RedisRepository> logger,
            IRedisConfiguration configuration)
        {
            _logger = logger;
        }

        public Task<T> GetStringItem<T>(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetStringItem<T>(RedisKey key, T value, long? expiryInMinutes = null)
        {
            throw new NotImplementedException();
        }
    }
}
