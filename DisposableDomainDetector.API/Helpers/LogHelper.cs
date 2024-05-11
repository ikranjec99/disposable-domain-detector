using Serilog;
using DisposableDomainDetector.API.Extensions;

namespace DisposableDomainDetector.API.Helpers
{
    public static class LogHelper
    {
        private static readonly string[] Headers = { "User-Agent", "X-Forwarder-For" };

        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            diagnosticContext.Set(nameof(httpContext.Connection.LocalIpAddress), httpContext.Connection.LocalIpAddress);
            diagnosticContext.Set(nameof(httpContext.Connection.RemoteIpAddress), httpContext.Connection.RemoteIpAddress);

            var request = httpContext.Request;

            diagnosticContext.Set(nameof(request.Host), request.Host);
            diagnosticContext.Set("RequestMethod", request.Method);
            diagnosticContext.Set("Resource", request.Path.Value);
            diagnosticContext.Set(nameof(request.Scheme), request.Scheme);
            diagnosticContext.Set(nameof(httpContext.Response.ContentType), httpContext.Response.ContentType);
            diagnosticContext.Set(nameof(httpContext.RequestAborted.IsCancellationRequested), httpContext.RequestAborted.IsCancellationRequested);
            diagnosticContext.Set("AvailableHeaders", request.Headers.Keys);

            foreach (string header in request.Headers.Keys.Where(header => Headers.Contains(header)))
                diagnosticContext.Set(header, request.Headers.GetValue(header));

            foreach (var route in request.RouteValues)
                diagnosticContext.Set(route.Key, route.Value);
        }
    }
}
