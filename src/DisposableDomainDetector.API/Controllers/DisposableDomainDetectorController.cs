using DisposableDomainDetector.Core.Business.Interfaces;
using DisposableDomainDetector.Core.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DisposableDomainDetector.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/disposable-domain-detector")]
    [ApiController]
    public class DisposableDomainDetectorController : ControllerBase
    {
        private readonly IDisposableDomainHandler _disposableDomainHandler;

        public DisposableDomainDetectorController(IDisposableDomainHandler disposableDomainHandler)
        {
            _disposableDomainHandler = disposableDomainHandler;
        }

        [HttpPost]
        public async Task<IActionResult> GetIsDisposableEmailDomain([FromBody] GetIsDisposableEmailDomainRequest request)
            => Ok(await _disposableDomainHandler.HasDisposableDomain(request.Email));
    }
}
