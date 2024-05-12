using System.ComponentModel.DataAnnotations;

namespace DisposableDomainDetector.API.Settings
{
    public class AppSettings
    {
        [Required]
        public required DisposableDomainElement DisposableDomains { get; set; }

        [Required]
        public required RedisElement Redis { get; set; }
    }
}
