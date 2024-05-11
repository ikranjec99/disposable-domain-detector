using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace DisposableDomainDetector.API.Providers
{
    public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
    {
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            switch (context.ErrorCode)
            {
                case "UnsupportedApiVersion":
                    context = new ErrorResponseContext(context.Request, context.StatusCode, context.ErrorCode,
                        "Selected API version is not supported", context.MessageDetail);
                    break;
            }

            return base.CreateResponse(context);
        }
    }
}
