namespace DisposableDomainDetector.Core.Business.Extensions
{
    public static class StringExtensions
    {
        private const int MaxLogLength = 2000;

        public static string TrimForLog(this string input) =>
            !string.IsNullOrEmpty(input) && input.Length > MaxLogLength ? input[..MaxLogLength] : input;
    }
}
