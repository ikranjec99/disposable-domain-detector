using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using DisposableDomainDetector.API.Helpers;
using DisposableDomainDetector.API.Filters;
using DisposableDomainDetector.API.Providers;
using DisposableDomainDetector.API.Settings;
using DisposableDomainDetector.Core.Configuration;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Interfaces;
using DisposableDomainDetector.Core.DataAccess.DisposableDomains.Implementations;
using DisposableDomainDetector.Core.Business.Interfaces;
using DisposableDomainDetector.Core.Business.Implementations;
using Destructurama;

namespace DisposableDomainDetector.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddBusinessLayer(this IServiceCollection services) =>
            services
                .AddScoped<IDisposableDomainHandler, DisposableDomainHandler>();

        private static IServiceCollection AddDataAccessLayer(this IServiceCollection services) =>
            services
                .AddSingleton<IDisposableDomainService, DisposableDomainService>();

        private static IServiceCollection AddSettings(this IServiceCollection services, AppSettings appSettings)
        {
            if (appSettings is null)
                throw new ArgumentNullException(nameof(appSettings));

            services.AddSingleton<IDisposableDomainConfiguration>(appSettings.DisposableDomains);

            return services;
        }

        private static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Information)
                .MinimumLevel.Override(nameof(System), LogEventLevel.Information)
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Destructure.With<NullIgnoringDestructuringPolicy>()
                .Destructure.UsingAttributes()
                .CreateLogger();

            return services;
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration, AppSettings appSettings)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Contact = new OpenApiContact
                    {
                        Name = "Ivan Kranjec"
                    },
                    Title = "Disposable Domain Detector API",
                    Version = "1.0"
                });
            });
            services.AddLogging(options => options.SetMinimumLevel(LogLevel.Debug));

            services
            .AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
                options.RespectBrowserAcceptHeader = true;
                options.Filters.Add(new ExceptionFilter(Log.Logger));
            });

            services
                .AddCors(options => options.AddPolicy(nameof(CorsPolicy),
                         builder =>
                         {
                             builder.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader();
                         }));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();

            services
                .AddApiVersioning(setup =>
                {
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                    setup.DefaultApiVersion = new ApiVersion(1, 0);
                    setup.ErrorResponses = new ApiVersioningErrorResponseProvider();
                })
                .AddRouting(options => options.LowercaseUrls = true)
                .AddSettings(appSettings)
                .AddBusinessLayer()
                .AddDataAccessLayer()
                .AddHttpContextAccessor();
        }
    }
}
