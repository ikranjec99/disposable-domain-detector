using DisposableDomainDetector.Core.Business.Exceptions;
using DisposableDomainDetector.Core.Business.Interfaces;
using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.Extensions;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Constants;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Interfaces;
using DisposableDomainDetector.Core.DataAccess.Redis.Exceptions;
using DisposableDomainDetector.Core.DataAccess.Redis.Interfaces;
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
        private readonly IRedisRepository _redisRepository;

        public DisposableDomainHandler(
            IDisposableDomainConfiguration configuration,
            IDisposableDomainService disposableDomainService,
            ILogger<IDisposableDomainHandler> logger,
            IMemoryCache memoryCache,
            IRedisRepository redisRepository)
        {
            _configuration = configuration;
            _disposableDomainService = disposableDomainService;
            _logger = logger;
            _memoryCache = memoryCache;
            _redisRepository = redisRepository;
        }

        public async Task<bool> HasDisposableDomain(string email)
        {
            try
            {
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
            string lowercaseDomain = email.Split('@').ElementAtOrDefault(1).ToLower();
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
                    string cachedResult = await _redisRepository.GetStringItem<string>(Cache.DisposableDomainsKey);
                    InMemoryCacheSync(cachedResult);
                    return cachedResult;
                }

                return result;
            }
            catch (RedisKeyNotFoundException)
            {
                _logger.LogRedisKeyNotFound(Cache.DisposableDomainsKey);
                return await CacheSync(CacheSyncMode.Complete);
            }
            catch (RedisNotAvailableException)
            {
                _logger.LogRedisNotAvailable();
                return await CacheSync(CacheSyncMode.InMemory);
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(FetchDisposableDomains));
                throw;
            }
        }

        private async Task<string> CacheSync(CacheSyncMode mode)
        {
            try
            {
                string result = await _disposableDomainService.GetDisposableDomains();

                if (mode == CacheSyncMode.InMemory)
                    InMemoryCacheSync(result);

                if (mode == CacheSyncMode.Complete)
                {
                    InMemoryCacheSync(result);
                    await _redisRepository.SetStringItem(Cache.DisposableDomainsKey, result, _configuration.ExpiryInMinutes);
                }

                return result;
            }
            catch (RedisNotAvailableException)
            {
                _logger.LogRedisNotAvailable();
                return _memoryCache.Get<string>(Cache.DisposableDomainsKey);
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(CacheSync));
                throw;
            }
        }

        private void InMemoryCacheSync(string result)
        {
            var expiration = DateTimeOffset.Now.AddMinutes(_configuration.ExpiryInMinutes);
            _memoryCache.Set(Cache.DisposableDomainsKey, result, expiration);
        }
    }
}
