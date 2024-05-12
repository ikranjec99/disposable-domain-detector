using System.Runtime.Serialization;

namespace DisposableDomainDetector.Core.DataAccess.Redis.Exceptions
{
    [Serializable]
    public class RedisException : Exception
    {
        protected RedisException(string message) : base(message) { }

        protected RedisException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
