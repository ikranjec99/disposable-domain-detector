using DisposableDomainDetector.Core.Constants;
using DisposableDomainDetector.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace DisposableDomainDetector.Core.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogRedisNotAvailable(this ILogger logger)
            => logger.LogInformation(DisposableDomainDetectorEvent.RedisNotAvailable.ToEventId(), "Redis not available");

        public static void LogRedisKeyNotFound(this ILogger logger, string key)
            => logger.LogInformation(DisposableDomainDetectorEvent.RedisKeyNotFound.ToEventId(), "Redis key: {Key} not found", key);
    }
}
