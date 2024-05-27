namespace DisposableDomainDetector.Core.Configuration
{
    public interface IDisposableDomainConfiguration : IServiceConfiguration
    {
        public long ExpiryInMinutes { get; set; }
    }
}
