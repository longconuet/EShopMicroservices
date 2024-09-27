using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Data;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Host.UseSerilog(SeriLogger.Configure);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
