using StackExchange.Redis;
using DisposableDomainDetector.Core.DataAccess.Redis.Exceptions;

namespace DisposableDomainDetector.Core.DataAccess.Redis.Interfaces
{
    public interface IRedisRepository
    {
        /// <summary>
        /// Get the deserialized value of key. Stored value must be a JSON string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>The deserialized value of the key</returns>
        /// <exception cref="RedisKeyNotFoundException">Thrown when the key does not exist or if there is no value of key.</exception>
        /// <exception cref="RedisNotAvailableException">Thrown when Redis service is not available.</exception>
        public Task<T> GetStringItem<T>(RedisKey key);

        /// <summary>
        /// Sets the key to hold the serialized value.
        /// If key already has a value, then it overwrites already existing value.
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="value">The value to serialize and set.</param>
        /// <param name="expiryInMinutes">Custom value of TTL, for some key.</param>
        public Task<bool> SetStringItem<T>(RedisKey key, T value, long? expiryInMinutes = null);
    }
}
