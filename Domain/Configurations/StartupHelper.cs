using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Domain.Configurations
{
    public static class StartupHelper
    {

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticSearchUri = configuration["ElasticSearch:Uri"];
            var password = configuration["ElasticSearch:Password"];

            if (string.IsNullOrEmpty(elasticSearchUri) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("ElasticSearch configuration is missing.");
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUri))
                {
                    IndexFormat = "ecommerce-{0:yyyy.MM.dd}",
                    AutoRegisterTemplate = true,
                    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", password),
                })
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            return services;
        }
    }
}

