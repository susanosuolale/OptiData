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

var builder = WebApplication.CreateBuilder(args);

// Add production SQL database connection (PostgreSQL for Render deployment)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddTransient<INotificationService, SignalRNotificationService>();

var app = builder.Build();

// Run the database seeder to create the Test User and fake usage data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
