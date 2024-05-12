using System.ComponentModel.DataAnnotations;

namespace DisposableDomainDetector.API.Settings
{
    public class AppSettings
    {
        [Required]
        public required DisposableDomainElement DisposableDomains { get; set; }
    }
}
