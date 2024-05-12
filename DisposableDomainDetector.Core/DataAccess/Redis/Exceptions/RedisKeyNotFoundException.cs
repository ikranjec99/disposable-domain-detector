using System.Runtime.Serialization;

namespace DisposableDomainDetector.Core.DataAccess.Redis.Exceptions
{
    public class RedisKeyNotFoundException : RedisException
    {
        public RedisKeyNotFoundException() : base("Redis key not found") { }

        public RedisKeyNotFoundException(string key) : base($"Redis key: {key} not found") { }

        protected RedisKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
