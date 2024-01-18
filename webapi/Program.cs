using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using webapi.Database;
using webapi.Utilities.Common;

// WebApplicationBuilder object
var builder = WebApplication.CreateBuilder(args);

// Load env variables
Env.Load("myEnv.env"); // Includes the connection string for your db
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var isTestEnv = Environment.GetEnvironmentVariable("ASPNET__isTestEnvironment"); // set from test ApiFactory

if(isTestEnv != null)
{
    connectionString = Environment.GetEnvironmentVariable("ASPNET__TempDbConnectionString"); // set from test ApiFactory
}

// Add DbContext
builder.Services.AddDbContext<AuctionContext>(options =>
    options.UseSqlServer(connectionString!));

// Add Http context accessor
builder.Services.AddHttpContextAccessor();

// Get configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Startup helper object
var startupHelper = new StartupHelper(builder, configuration);

// Add services to Dependency Injection container
startupHelper.AddServices();

// Cors Policy, authentication, authorization
startupHelper.ConfigureServerSecurity();

// Mapper configuration
startupHelper.ConfigureAutomapper();

// Logger configuration
startupHelper.ConfigureLogger();

// Https configuration
startupHelper.ConfigureHttps();

// Build app
var app = builder.Build();

// Admin configuration
startupHelper.ConfigureAdmin(app);

// Middleware configuration
startupHelper.ConfigureMiddleware(app);

app.MapControllers();

app.Run();