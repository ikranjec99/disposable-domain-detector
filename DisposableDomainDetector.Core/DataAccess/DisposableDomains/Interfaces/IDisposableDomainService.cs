namespace DisposableDomainDetector.Core.DataAccess.DisposableDomains.Interfaces
{
    public interface IDisposableDomainService
    {
        /// <summary>
        /// Get raw disposable domains from Github repository.
        /// </summary>
        /// <returns></returns>
        Task<string> GetDisposableDomains();
    }
}
