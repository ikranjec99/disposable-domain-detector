using Serilog;

namespace DisposableDomainDetector.API.Extensions
{
    public static class HostBuilderExtensions
    {
        public static void Configure(this IHostBuilder builder) =>
            builder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            });
    }
}
