using DisposableDomainDetector.Core.Business.Constants;
using DisposableDomainDetector.Core.Business.Exceptions;
using DisposableDomainDetector.Core.Business.Extensions;
using DisposableDomainDetector.Core.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DisposableDomainDetector.Core.DataAccess
{
    public abstract class HttpService
    {
        protected HttpService(IServiceConfiguration configuration, ILogger<HttpService> logger, HttpClient httpClient)
        {
            Client = httpClient;
            Client.BaseAddress = new Uri(configuration.BaseUrl);
            Client.Timeout = configuration.Timeout.HasValue ? TimeSpan.FromMilliseconds(configuration.Timeout.Value) : Client.Timeout;
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Culture = CultureInfo.InvariantCulture
            };
            Logger = logger;
            ResponseLogged = configuration.ResponseLogged ?? true;
        }

        protected HttpService(IServiceConfiguration configuration, ILogger<HttpService> logger, HttpMessageHandler httpMessageHandler = null)
        {
            Client = httpMessageHandler is null ? new HttpClient() : new HttpClient(httpMessageHandler);
            Client.BaseAddress = new Uri(configuration.BaseUrl);
            Client.Timeout = configuration.Timeout.HasValue ? TimeSpan.FromMilliseconds(configuration.Timeout.Value) : Client.Timeout;
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Culture = CultureInfo.InvariantCulture
            };
            Logger = logger;
            ResponseLogged = configuration.ResponseLogged ?? true;
        }

        protected HttpClient Client { get; }

        protected ILogger Logger { get; }

        protected JsonSerializerSettings JsonSerializerSettings { get; }

        protected bool ResponseLogged { get; }

        protected virtual async Task<TResponse> Call<TResponse>(HttpRequestMessage request)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await Client.SendAsync(request);
            stopwatch.Stop();

            Logger.LogDebug("{Method} {RequestPath} took {ResponseTime}ms, status code: {ResponseStatusCode}", request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, (int)response.StatusCode);

            string responseText = string.Empty;

            try
            {
                if (response?.Content is null)
                    throw new InvalidDataException("Response is null or empty");

                responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogDebug("Response from {RequestPath}: {RequestBody}", request.RequestUri, responseText.TrimForLog());
                    throw new HttpRequestStatusException(response.StatusCode, responseText);
                }

                if (ResponseLogged && typeof(TResponse) != typeof(object))
                    Logger.LogDebug("Response from {RequestPath}: {@ResponseBody}", request.RequestUri, JsonConvert.DeserializeObject<TResponse>(responseText));

                return JsonConvert.DeserializeObject<TResponse>(responseText);
            }
            catch (JsonReaderException)
            {
                return (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { });
            }
            catch (HttpRequestStatusException e)
            {
                Logger.LogDebug(e, "HTTP request failed");
                throw;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "{Method} failed: Response Text: {ResponseText}, Exception: {Message}", nameof(Call), responseText, e.Message);
                throw new InvalidDataException("Could not deserialize the response", e);
            }
        }

        protected virtual async Task Call(HttpRequestMessage request)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = await Client.SendAsync(request);
            stopwatch.Stop();

            Logger.LogDebug("{Method} {RequestPath} took {ResponseTime}ms, status code: {ResponseStatusCode}", request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, (int)response.StatusCode);

            string responseText = string.Empty;

            try
            {
                if (response?.Content is null)
                    throw new InvalidDataException("Response is null or empty");

                responseText = await response.Content.ReadAsStringAsync();

                if (ResponseLogged)
                    Logger.LogDebug("Response from {RequestPath}: {ResponseBody}", request.RequestUri, responseText.TrimForLog());

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestStatusException(response.StatusCode, responseText);
            }
            catch (HttpRequestStatusException e)
            {
                Logger.LogDebug(e, "HTTP request failed");
                throw;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "{Method} failed: ResponseText: {ResponseText}, Exception: {Message}", nameof(Call), responseText, e.Message);
                throw new InvalidDataException("Could not deserialize the response", e); ;
            }
        }

        protected virtual async Task<TResponse> Get<TResponse>(string requestPath, IDictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>();

            using var request = new HttpRequestMessage(HttpMethod.Get, requestPath);

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            return await Call<TResponse>(request);
        }

        protected virtual async Task Patch<TRequest>(string requestPath, TRequest requestBody, IDictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>();

            using var request = new HttpRequestMessage(HttpMethod.Patch, requestPath);
            string requestText = JsonConvert.SerializeObject(requestBody, JsonSerializerSettings);
            request.Content = new StringContent(requestText, Encoding.UTF8, MediaType.Json);

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            await Call(request);
        }

        protected virtual async Task<TResponse> Post<TRequest, TResponse>(string requestPath, TRequest requestBody, IDictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>();

            using var request = new HttpRequestMessage(HttpMethod.Post, requestPath);

            if (requestBody is MultipartFormDataContent)
                request.Content = requestBody as MultipartFormDataContent;
            else
            {
                string requestText = JsonConvert.SerializeObject(requestBody, JsonSerializerSettings);
                request.Content = new StringContent(requestText, Encoding.UTF8, MediaType.Json);
            }

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            return await Call<TResponse>(request);
        }

        protected virtual async Task Put<TRequest>(string requestPath, TRequest requestBody, IDictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>();

            using var request = new HttpRequestMessage(HttpMethod.Put, requestPath);
            string requestText = JsonConvert.SerializeObject(requestBody, JsonSerializerSettings);
            request.Content = new StringContent(requestText, Encoding.UTF8, MediaType.Json);

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            await Call(request);
        }

        /// <summary>
        /// Joins all fragments with a forward slash separator.
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public static string CombineUrlPath(params string[] fragments)
        {
            if (fragments is null || fragments.Contains(null)) 
                throw new ArgumentNullException(nameof(fragments));

            return string.Join("/", fragments.Select(x => x.Trim('/')));
        }

        /// <summary>
        /// Builds an Uri by joining all fragments with a forward slash separator to the base Uri.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public static Uri ConstructUri(Uri baseUri, params string[] fragments)
        {
            if (baseUri is null)
                throw new ArgumentNullException(nameof(baseUri));

            return new Uri(baseUri, CombineUrlPath(fragments));
        }

        /// <summary>
        /// Builds an Uri by joining all fragments with a forward slash separator.
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public static Uri ConstructUri(params string[] fragments) => new Uri(CombineUrlPath(fragments));
    }
}
