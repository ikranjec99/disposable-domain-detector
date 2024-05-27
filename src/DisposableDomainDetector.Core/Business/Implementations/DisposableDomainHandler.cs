using DisposableDomainDetector.Core.Business.Exceptions;
using DisposableDomainDetector.Core.Business.Interfaces;
using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Constants;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Interfaces;
using DisposableDomainDetector.Core.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DisposableDomainDetector.Core.Business.Implementations
{
    public class DisposableDomainHandler : IDisposableDomainHandler
    {
        private readonly IDisposableDomainService _disposableDomainService;
        private readonly IDisposableDomainConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public DisposableDomainHandler(
            IDisposableDomainConfiguration configuration,
            IDisposableDomainService disposableDomainService,
            ILogger<IDisposableDomainHandler> logger,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _disposableDomainService = disposableDomainService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> HasDisposableDomain(string email)
        {
            try
            {
                _logger.LogTryHasDisposableDomain(GetDomain(email));
                string result = await FetchDisposableDomains();

                return IsDisposableDomain(result, email);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(HasDisposableDomain));
                throw;
            }
        }

        private bool IsDisposableDomain(string domainList, string email)
        {
            string lowercaseDomain = GetDomain(email);
            var disposableDomains = domainList.Split("\n").ToList();

            return disposableDomains.Contains(lowercaseDomain);
        }

        private async Task<string> FetchDisposableDomains()
        {
            try
            {
                string result = _memoryCache.Get<string>(Cache.DisposableDomainsKey);

                if (result is null)
                {
                    string cachedResult = await _disposableDomainService.GetDisposableDomains();
                    InMemoryCacheSync(cachedResult);
                    return cachedResult;
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(FetchDisposableDomains));
                throw;
            }
        }

        private string GetDomain(string email) => email.Split('@').ElementAtOrDefault(1).ToLower();

        private void InMemoryCacheSync(string result)
        {
            string cacheKey = Cache.DisposableDomainsKey;
            _logger.LogTryInMemoryCacheSync(cacheKey);
            var expiration = DateTimeOffset.Now.AddMinutes(_configuration.ExpiryInMinutes);
            _memoryCache.Set(cacheKey, result, expiration);
        }
    }
}
