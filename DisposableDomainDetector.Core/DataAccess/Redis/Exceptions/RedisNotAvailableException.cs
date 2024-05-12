using System.Runtime.Serialization;

namespace DisposableDomainDetector.Core.DataAccess.Redis.Exceptions
{
    public class RedisNotAvailableException : RedisException
    {
        public RedisNotAvailableException() : base("Redis not available") { }

        protected RedisNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }   
}
