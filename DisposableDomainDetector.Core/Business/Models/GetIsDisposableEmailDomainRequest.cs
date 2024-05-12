using Destructurama.Attributed;

namespace DisposableDomainDetector.Core.Business.Models
{
    public class GetIsDisposableEmailDomainRequest
    {
        [LogMasked]
        public required string Email { get; set; }
    }
}
