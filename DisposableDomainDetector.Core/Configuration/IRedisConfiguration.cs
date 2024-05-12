namespace DisposableDomainDetector.Core.Configuration
{
    public interface IRedisConfiguration
    {
        int ConnectionTimeout { get; }

        int ExpiryInMinutes { get; }

        string KeyPrefix { get; }

        string MasterName { get; }

        string SentinelHosts { get; }

        int SyncTimeout { get; }
    }
}
