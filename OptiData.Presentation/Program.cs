using Microsoft.EntityFrameworkCore;
using OptiData.Infrastructure.Data;
using OptiData.Application.Interfaces;
using OptiData.Application.Services;
using OptiData.Infrastructure.Repositories;
using OptiData.Infrastructure.Jobs;
using OptiData.Infrastructure.MachineLearning;
using OptiData.Infrastructure.Services;
using OptiData.Application.Bundles.Commands.OptimizeBundles;
using OptiData.Domain.Enums;
using Hangfire;
using Hangfire.PostgreSql;
using OptiData.Presentation.Hubs;
using OptiData.Presentation.Services;

// Fix for Render/Linux "inotify instances limit reached" error.
// Cloud containers are immutable, so we don't need to watch appsettings.json for live changes anyway.
Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");
// This disables inotify entirely across the whole app (including Razor view static file versioning watchers)
Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");

var builder = WebApplication.CreateBuilder(args);

// Helper to parse Render's postgres:// URL into standard ADO.NET format
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Fallback just in case the user set the variable as DATABASE_URL instead of ConnectionStrings__DefaultConnection
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("localhost"))
{
    var renderDbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(renderDbUrl))
    {
        connectionString = renderDbUrl;
    }
}

// Aggressively sanitize (users often accidentally paste quotes or spaces in cloud dashboards)
if (!string.IsNullOrEmpty(connectionString))
{
    connectionString = connectionString.Trim(' ', '"', '\'', '\n', '\r');
    
    if (connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        var port = uri.Port > 0 ? uri.Port : 5432;
        connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true;";
    }
}

// Add production SQL database connection (PostgreSQL for Render deployment)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Application and Infrastructure Services
builder.Services.AddScoped<IBundleOptimizationService, BundleOptimizationService>();
builder.Services.AddScoped<IDataBundleRepository, DataBundleRepository>();
builder.Services.AddScoped<IDataPredictionService, DataPredictionService>();
builder.Services.AddHttpClient<IPaymentService, PaystackPaymentService>();
builder.Services.AddScoped<ICurrentUserService, MockCurrentUserService>();
builder.Services.AddScoped<IPurchaseSchedulerService, HangfirePurchaseScheduler>();
// creates fully configured HttpClient and injects it into OpenAIAssistantService
builder.Services.AddHttpClient<IAssistantService, OpenAIAssistantService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OptimizeBundlesCommand).Assembly));

// Register all Telecom Providers so the background job can download all their bundles
builder.Services.AddScoped<ITelecomProviderService, MtnTelecomProvider>();
builder.Services.AddScoped<ITelecomProviderService, AirtelTelecomProvider>();
builder.Services.AddScoped<ITelecomProviderService, GloTelecomProvider>();
builder.Services.AddScoped<ITelecomProviderService, NineMobileTelecomProvider>();

// Configure Hangfire for Background Jobs using PostgreSQL
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddTransient<INotificationService, SignalRNotificationService>();

var app = builder.Build();

// Run the database migrations and seeder to create the tables, Test User, and fake usage data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Automatically apply Entity Framework migrations on startup (creates the tables in Postgres)
    await context.Database.MigrateAsync();
    
    await DataSeeder.SeedAsync(context);
}

// Enable the Hangfire Dashboard UI
app.UseHangfireDashboard();

// Since our data is hardcoded for the portfolio, we only need to run this job exactly once 
// when the application starts, rather than every midnight.
BackgroundJob.Enqueue<FetchBundlesJob>(job => job.ExecuteAsync());

// Schedule the simulator to run every 5 minutes to generate fake data usage
RecurringJob.AddOrUpdate<DataUsageSimulatorJob>("data-usage-simulator", job => job.SimulateUsageAsync(), "*/5 * * * *");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// creates url and maps that url to the NotificationHub class
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
