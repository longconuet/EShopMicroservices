using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace Common.Logging;

public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, configuration) =>
    {
        var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

        configuration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl!))
            {
                AutoRegisterTemplate = true,            // Automatically register the ECS template
                //IndexFormat = "ecs-logs-{0:yyyy.MM}",   // Index name format (you can change this)
                IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",   // Index name format (you can change this)
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
            .Enrich.WithProperty("Enviroment", context.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .ReadFrom.Configuration(context.Configuration);
    };
}