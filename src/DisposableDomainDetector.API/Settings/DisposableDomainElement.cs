using DisposableDomainDetector.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DisposableDomainDetector.API.Settings
{
    public class DisposableDomainElement : ServiceElement, IDisposableDomainConfiguration
    {
        [Required]
        public long ExpiryInMinutes { get; set; }
    }
}
