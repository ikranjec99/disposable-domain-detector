using DisposableDomainDetector.Core.Constants;
using Microsoft.Extensions.Logging;

namespace DisposableDomainDetector.Core.Extensions
{
    public static class DisposableDomainDetectorExtensions
    {
        public static EventId ToEventId(this DisposableDomainDetectorEvent disposableDomainDetectorEvent)
            => new EventId((int)disposableDomainDetectorEvent, disposableDomainDetectorEvent.ToString());
    }
}
