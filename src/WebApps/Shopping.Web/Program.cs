using Common.Logging;
using Shopping.Web.Services;
using Shopping.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<LoggingDelegatingHandler>();


builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    {
        c.BaseAddress = new Uri("https://localhost:5050"); // builder.Configuration.GetValue<string>("CatalogAPI")!)
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();


builder.Services.AddControllersWithViews();

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
