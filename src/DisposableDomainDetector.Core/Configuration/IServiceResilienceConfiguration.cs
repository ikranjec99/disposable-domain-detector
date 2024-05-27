namespace DisposableDomainDetector.Core.Configuration
{
    public interface IServiceResilienceConfiguration
    {
        /// <summary>
        /// Global request timeout in seconds. The timeout is applied to the operation with retries. For applying timeout for individual calls use <see cref="RetryTimeout"/>
        /// </summary>
        public int? Timeout { get; }

        /// <summary>
        /// Retry count used for resilience.
        /// </summary>
        public int? RetryCount { get; }

        /// <summary>
        /// Delay between retries defined in miliseconds.
        /// </summary>
        public int? RetryDelayMiliseconds { get;  }

        /// <summary>
        /// Timeout applied to each individual retry call in miliseconds.
        /// </summary>
        public int? RetryTimeoutMiliseconds { get; }
    }
}
