using System.ComponentModel.DataAnnotations;

namespace DisposableDomainDetector.API.Settings
{
    public class ServiceElement
    {
        [Url]
        public string BaseUrl { get; set; }

        public int? Timeout { get; set; }

        public bool? ResponseLogged { get; set; }

        public int? RetryCount { get; set; }

        public int? RetryDelayMiliseconds { get; set; }

        public int? RetryTimeoutMiliseconds { get; set; }
    }
}
