using webapi.Services;
using webapi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using webapi.Contracts.Policies;
using webapi.Database;
using webapi.Models;
using webapi.Repository;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddScoped<IAuctionContext, AuctionContext>();
builder.Services.AddDbContext<AuctionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Service, Repository, and Controller helper
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ControllerHelper>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Authorization
builder.Services.AddAuthorization(options => options.AddPolicy(Policies.Admin, policy => policy.RequireClaim("IsSuperuser", "True")));

// AutoMapper configuration
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

var mapper = config.CreateMapper();

builder.Services.AddSingleton(mapper);

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

    if (adminUser == null)
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
