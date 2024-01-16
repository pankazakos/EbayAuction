using AutoMapper;
using webapi.Database;
using webapi.Repository.Interfaces;
using webapi.Repository;
using webapi.Services.Interfaces;
using webapi.Services;
using webapi.Utilities.ControllerUtils;
using webapi.Utilities.MappingUtils;
using Microsoft.AspNetCore.Authorization;
using webapi.Utilities.AuthorizationUtils.PolicyUtils;
using contracts.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using webapi.Models;
using webapi.Utilities.AuthorizationUtils.PasswordUtils;

namespace webapi.Utilities.Common
{
    public class StartupHelper
    {
        private readonly WebApplicationBuilder _builder;
        private readonly IConfiguration _configuration;
        private readonly string AllowAnyOrigin = "AllowAnyOrigin";

        public StartupHelper(WebApplicationBuilder builder, IConfiguration configuration)
        {
            _builder = builder;
            _configuration = configuration;
        }

        public void AddServices()
        {
            _builder.Services.AddControllers();
            _builder.Services.AddScoped<IAuctionContext, AuctionContext>();
            _builder.Services.AddScoped<IUserService, UserService>();
            _builder.Services.AddScoped<IItemService, ItemService>();
            _builder.Services.AddScoped<IUserRepository, UserRepository>();
            _builder.Services.AddScoped<IItemRepository, ItemRepository>();
            _builder.Services.AddScoped<ICategoryService, CategoryService>();
            _builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            _builder.Services.AddScoped<IBidService, BidService>();
            _builder.Services.AddScoped<IBidRepository, BidRepository>();
            _builder.Services.AddScoped<IControllerHelper, ControllerHelper>();
            _builder.Services.AddScoped<IAuthorizationHandler, SelfUserAuthorizationHandler>();
            _builder.Services.AddScoped<IAuthorizationHandler, ItemOwnerAuthorizationHandler>();
        }

        private void ConfigureCorsPolicy()
        {
            _builder.Services.AddCors(options => options.AddPolicy(AllowAnyOrigin, tempBuilder => tempBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
        }

        private void ConfigureAuthentication()
        {
            _builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _builder.Configuration["Jwt:Issuer"],
                    ValidAudience = _builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_builder.Configuration["Jwt:Key"]!))
                };
            });
        }

        private void ConfigureAuthorization()
        {
            _builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy => policy.RequireClaim("IsSuperuser", "True"));
                options.AddPolicy(Policies.SelfUser, policy => policy.AddRequirements(new SelfUserRequirement()));
                options.AddPolicy(Policies.ItemOwner, policy => policy.AddRequirements(new ItemOwnerRequirement()));
            });
        }

        public void ConfigureServerSecurity()
        {
            ConfigureCorsPolicy();
            ConfigureAuthentication();
            ConfigureAuthorization();
        }

        public void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            _builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            _builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
        }

        public void ConfigureHttps()
        {
            _builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.ListenAnyIP(7068, listenOptions =>
                {
                    listenOptions.UseHttps("../ebayauction.pfx", "ebaypass");
                });
            });
        }

        public void ConfigureAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            var mapper = config.CreateMapper();

            _builder.Services.AddSingleton(mapper);
        }

        public async void ConfigureAdmin(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<IAuctionContext>();
                var userService = services.GetRequiredService<IUserService>();

                // Check if the admin user already exists and create if he doesn't
                const string adminUsername = "admin";
                const string adminPassword = "admin";

                var adminUser = await userService.GetByUsername(adminUsername);

                if (adminUser is null)
                {
                    var salt = PasswordHelper.GenerateSalt();
                    var hashedPassword = PasswordHelper.HashPassword(adminPassword, salt);

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
        }

        public void ConfigureMiddleware(WebApplication app)
        {
            app.UseCors(AllowAnyOrigin);

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
        }
    }
}
