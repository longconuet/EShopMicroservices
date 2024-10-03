using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Shopping.Web.Services;
using Shopping.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<LoggingDelegatingHandler>();


builder.Services.AddHttpClient<ICatalogService, CatalogService>("catalog-api", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:CatalogApiUrl")!);
        //c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CatalogAPI")!);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    //.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, retryA ttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    //.AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(20)));
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddControllersWithViews();

// config serilog
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{builder.Configuration.GetValue<string>("ApiSettings:CatalogApiUrl")!}index.html"), name: "catalog_api", tags: ["url", "liveness"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() 
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30));
}
