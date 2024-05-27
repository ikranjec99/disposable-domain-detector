namespace DisposableDomainDetector.API.Extensions
{
    public static class HeaderDictionaryExtensions
    {
        public static string GetValue(this IHeaderDictionary headers, string headerKey)
        {
            string headerValue = headers
                .FirstOrDefault(x => string.Equals(x.Key, headerKey, StringComparison.CurrentCultureIgnoreCase)).Value
                .ToString();

            return string.IsNullOrEmpty(headerValue) ? null : headerValue;
        }
    }
}
