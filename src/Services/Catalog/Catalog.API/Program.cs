using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Data;
using Elastic.Channels;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMarten(option =>
{
    option.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    .ReadFrom.Configuration(builder.Configuration)
//    .WriteTo.Console() // Optional: Log to console
//    .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]!))
//    {
//        AutoRegisterTemplate = true,            // Automatically register the ECS template
//        IndexFormat = "ecs-logs-{0:yyyy.MM}",   // Index name format (you can change this)
//        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
//        NumberOfShards = 2,
//        NumberOfReplicas = 1
//    })
//    .CreateLogger();

//var localHostElasticURL = builder.Configuration["ElasticConfiguration:Uri"]!;
//string indexFormat = "stackOverFlowTestindex";
//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    .WriteTo.Elasticsearch(new[] { new Uri(localHostElasticURL) },
//    options =>
//    {
//        options.DataStream = new DataStreamName(indexFormat);
//        options.TextFormatting = new EcsTextFormatterConfiguration();
//        options.BootstrapMethod = BootstrapMethod.Failure;
//        options.ConfigureChannel = channelOptions =>
//        {
//            channelOptions.BufferOptions = new BufferOptions();
//        };
//    },
//    configureTransport =>
//    {
//        configureTransport.ServerCertificateValidationCallback((_, _, _, _) => true);
//    })
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

// Replace default logger
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]!))
        {
            AutoRegisterTemplate = true,            // Automatically register the ECS template
            //IndexFormat = "ecs-logs-{0:yyyy.MM}",   // Index name format (you can change this)
            IndexFormat = $"applogs-{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",   // Index name format (you can change this)
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
            NumberOfShards = 2,
            NumberOfReplicas = 1
        })
        .Enrich.WithProperty("Enviroment", context.HostingEnvironment.EnvironmentName)
        .ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
