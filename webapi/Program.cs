using webapi.Services;
using webapi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using webapi.Database;
using webapi.Models;
using webapi.Repository;
using Microsoft.AspNetCore.Mvc;

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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

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

builder.Services.AddAuthorization(options => options.AddPolicy("Admin", policy => policy.RequireClaim("IsSuperuser", "True")));

// Add Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ItemService>();

// Add controllerHelper
builder.Services.AddScoped<ControllerHelper>();

var app = builder.Build();

// Create admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AuctionContext>();
    var userManager = services.GetRequiredService<UserService>();

    // Check if the admin user already exists
    var adminUsername = "admin";
    var adminPassword = "admin";
    var adminUser = await userManager.GetByUsername(adminUsername);

    if (adminUser == null)
    {
        string salt = PasswordHelper.GenerateSalt();
        string hashedPassword = PasswordHelper.HashPassword(adminPassword, salt);

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
