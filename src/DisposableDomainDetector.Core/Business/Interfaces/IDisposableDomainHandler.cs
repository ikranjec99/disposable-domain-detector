namespace DisposableDomainDetector.Core.Business.Interfaces
{
    public interface IDisposableDomainHandler
    {
        /// <summary>
        /// Check if email is "disposable"
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> HasDisposableDomain(string email);
    }
}
