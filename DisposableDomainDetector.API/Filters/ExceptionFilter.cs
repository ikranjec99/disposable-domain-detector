using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Net.Http.Headers;
using MediaType = DisposableDomainDetector.Core.Business.Constants.MediaType;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DisposableDomainDetector.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly Serilog.ILogger _logger;

        public ExceptionFilter(Serilog.ILogger logger)
        {
            _logger = logger;
        }


        public void OnException(ExceptionContext context)
        {
            int statusCode = (int)MapExceptionToStatusCode(context.Exception);

            context.Result = new ObjectResult(context.Exception.Message)
            {
                StatusCode = statusCode,
                ContentTypes = new MediaTypeCollection { new MediaTypeHeaderValue(MediaType.TextPlain) },
            };

            if (statusCode >= 500)
                _logger.Error("Exception: {Exception}", JsonConvert.SerializeObject(context.Exception, Formatting.Indented));

            context.ExceptionHandled = true;
        }

        internal static HttpStatusCode MapExceptionToStatusCode(Exception e) => e switch
        {
            NotImplementedException => HttpStatusCode.NotImplemented,

            _ => HttpStatusCode.InternalServerError
        };
    }
}
