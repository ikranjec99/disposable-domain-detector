namespace DisposableDomainDetector.Core.Configuration
{
    public interface IServiceConfiguration : IServiceResilienceConfiguration
    {
        /// <summary>
        /// Service base Url.
        /// </summary>
        string BaseUrl { get; set; }

        /// <summary>
        /// If enabled response is logged. Enabled by default.
        /// </summary>
        bool? ResponseLogged { get; set; }
    }
}
