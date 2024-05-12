using DisposableDomainDetector.Core.Configuration;

namespace DisposableDomainDetector.API.Settings
{
    public class RedisElement : IRedisConfiguration
    {
        public int ConnectionTimeout { get; set; }

        public int ExpiryInMinutes { get; set; }

        public required string KeyPrefix { get; set; }

        public required string MasterName { get; set; }

        public required string SentinelHosts { get; set; }

        public int SyncTimeout { get; set; }
    }
}
