using Common.Logging;
using Serilog;
using Shopping.Web.Services;
using Shopping.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<LoggingDelegatingHandler>();


builder.Services.AddHttpClient<ICatalogService, CatalogService>("eshop-api", c =>
    {
        //var url = builder.Configuration.GetValue<string>("ApiSettings:GatewayAddress")!;
        //c.BaseAddress = new Uri($"{url}/catalog-service/");
        c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CatalogAPI")!);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddControllersWithViews();

// config serilog
builder.Host.UseSerilog(SeriLogger.Configure);

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

app.Run();
