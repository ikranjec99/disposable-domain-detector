using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Constants;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Interfaces;
using Microsoft.Extensions.Logging;

namespace DisposableDomainDetector.Core.DataAccess.DisposableDomains.Implementations
{
    public class DisposableDomainService : HttpService, IDisposableDomainService
    {
        private readonly IDisposableDomainConfiguration _configuration;

        public DisposableDomainService(
            IDisposableDomainConfiguration configuration,
            ILogger<DisposableDomainService> logger,
            HttpMessageHandler httpMessageHandler = null)
            : base(configuration, logger, httpMessageHandler)
        {
            _configuration = configuration;
        }

        public async Task<string> GetDisposableDomains()
        {
            string url = $"{_configuration.BaseUrl}/{Files.PlainText}";

            string responseText = string.Empty;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await Client.SendAsync(request);
                return responseText = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Logger.LogDebug(e, "HTTP request failed");
                throw;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "{Method} failed: Response Text: {ResponseText}, Exception: {Message}", nameof(GetDisposableDomains), responseText, e.Message);
                throw new InvalidDataException("Could not deserialize the response", e);
            }
        }
    }
}
