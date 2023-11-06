using webapi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using webapi.Database;
using webapi.Models;
using webapi.Repository;
using AutoMapper;
using contracts.Policies;
using Microsoft.AspNetCore.Authorization;
using webapi.Utilities.ControllerUtils;
using webapi.Utilities.MappingUtils;
using webapi.Utilities.AuthorizationUtils.PolicyUtils;
using webapi.Utilities.AuthorizationUtils.PasswordUtils;
using Serilog;
using webapi.Utilities.ServiceUtils;


var builder = WebApplication.CreateBuilder(args);

// Get configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddScoped<IAuctionContext, AuctionContext>();
builder.Services.AddDbContext<AuctionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Add Http context accessor
builder.Services.AddHttpContextAccessor();

// Add Service, Repository, and Controller helper
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IControllerHelper, ControllerHelper>();

// Add Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, SelfUserAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ItemOwnerAuthorizationHandler>();

// CORS Policy
builder.Services.AddCors(options => options.AddPolicy("AllowAnyOrigin",
    tempBuilder => tempBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.Admin, policy => policy.RequireClaim("IsSuperuser", "True"));
    options.AddPolicy(Policies.SelfUser, policy => policy.AddRequirements(new SelfUserRequirement()));
    options.AddPolicy(Policies.ItemOwner, policy => policy.AddRequirements(new ItemOwnerRequirement()));
});

// AutoMapper configuration
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

var mapper = config.CreateMapper();

builder.Services.AddSingleton(mapper);

// logger configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

// configure Serilog logger
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddSingleton<IAppLogHelper, AppLogHelper>();


builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.ListenAnyIP(7068, listenOptions =>
    {
        listenOptions.UseHttps("../ebayauction.pfx", "ebaypass");
    });
});


// Build app
var app = builder.Build();

// Create admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<IAuctionContext>();
    var userService = services.GetRequiredService<IUserService>();

    // Check if the admin user already exists
    const string adminUsername = "admin";
    const string adminPassword = "admin";
    var adminUser = await userService.GetByUsername(adminUsername);

    if (adminUser is null)
    {
        var salt = PasswordHelper.GenerateSalt();
        var hashedPassword = PasswordHelper.HashPassword(adminPassword, salt);

        // Create a new user
        var admin = new User
        {
            Username = adminUsername,
            PasswordHash = hashedPassword,
            PasswordSalt = salt,
            FirstName = "First_Name",
            LastName = "Last_Name",
            LastLogin = DateTime.Now,
            DateJoined = DateTime.Now,
            Email = $"{adminUsername}@email.com",
            Country = "USA",
            Location = "Chicago",
            IsSuperuser = true,
            IsActive = true
        };

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
