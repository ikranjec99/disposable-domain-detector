using DisposableDomainDetector.Core.Constants;
using Microsoft.Extensions.Logging;

namespace DisposableDomainDetector.Core.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogTryHasDisposableDomain(this ILogger logger, string domain)
            => logger.LogInformation(DisposableDomainDetectorEvent.TryHasDisposableDomain.ToEventId(), "Trying to check if domain {Domain} is disposable", domain);

        public static void LogTryInMemoryCacheSync(this ILogger logger, string key)
            => logger.LogInformation(DisposableDomainDetectorEvent.TryInMemoryCacheSync.ToEventId(), "Trying to run in memory cache sync with cache key: {Key}", key);
    }
}
