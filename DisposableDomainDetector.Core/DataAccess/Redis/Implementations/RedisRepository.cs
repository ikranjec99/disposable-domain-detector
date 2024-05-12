using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.DataAccess.Redis.Exceptions;
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
            _expiryInMinutes = configuration.ExpiryInMinutes;

            try
            {
                var options = ConfigurationOptions.Parse(configuration.SentinelHosts);
                options.AbortOnConnectFail = true;
                options.AllowAdmin = true;
                options.ConnectTimeout = configuration.ConnectTimeout;
                options.SyncTimeout = configuration.SyncTimeout;
                options.ServiceName = configuration.MasterName;
                options.TieBreaker = string.Empty;

                _redisMultiplexer = ConnectionMultiplexer.Connect(options);
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(RedisRepository));
                throw new RedisNotAvailableException();
            }
        }

        public async Task<T> GetStringItem<T>(RedisKey key)
        {
            try
            {
                if (!await RedisDatabase.KeyExistsAsync(key))
                    throw new RedisKeyNotFoundException(key);

                var value = await RedisDatabase.StringGetAsync(key);

                if (!value.HasValue)
                    throw new RedisKeyNotFoundException();

                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            }
            catch (Exception e) when (e is not RedisKeyNotFoundException)
            {
                _logger.LogError(e, nameof(GetStringItem));
                throw new RedisNotAvailableException();
            }
        }

        public async Task<bool> SetStringItem<T>(RedisKey key, T value, long? expiryInMinutes = null)
        {
            try
            {
                string serializedValue = ServiceStack.Text.JsonSerializer.SerializeToString(value);
                TimeSpan timespan = TimeSpan.FromMinutes(expiryInMinutes ?? _expiryInMinutes);

                await RedisDatabase.StringSetAsync(key, serializedValue, timespan);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(SetStringItem));
                return false;
            }
        }
    }
}
