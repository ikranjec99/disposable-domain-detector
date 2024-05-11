namespace DisposableDomainDetector.Core.Configuration
{
    public interface IDisposableDomainServiceConfiguration : IServiceConfiguration
    {
        public long ExpiryInMinutes { get; set; }
    }
}
