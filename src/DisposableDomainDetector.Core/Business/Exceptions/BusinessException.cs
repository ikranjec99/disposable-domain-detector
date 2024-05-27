using System.Runtime.Serialization;

namespace DisposableDomainDetector.Core.Business.Exceptions
{
    [Serializable]
    public abstract class BusinessException : Exception
    {
        protected BusinessException() { }

        protected BusinessException(string message) : base(message) { }

        protected BusinessException(string message, Exception exception) : base(message, exception) { }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
