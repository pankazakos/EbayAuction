using Microsoft.EntityFrameworkCore;
using webapi.Database;
using DotNetEnv;
using webapi.Utilities.Common;

// WebApplicationBuilder object
var builder = WebApplication.CreateBuilder(args);

// Get configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Startup helper object
var startupHelper = new StartupHelper(builder, configuration);

// Load env variables
Env.Load("myEnv.env");
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Add DbContext
builder.Services.AddDbContext<AuctionContext>(options =>
    options.UseSqlServer(connectionString!));

// Add Http context accessor
builder.Services.AddHttpContextAccessor();

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