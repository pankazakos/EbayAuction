using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using webapi;
using webapi.Database;


namespace Api.IntegrationTests
{
    public class ApiFactory : WebApplicationFactory<IApiMarker>
    {
        private static readonly MsSqlContainer DbContainer = new MsSqlBuilder()
            .WithPassword("zhskalComplexPass12")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithExposedPort(1433)
            .WithEntrypoint()
            .Build();

        private static ApiFactory? _instance = null;

        private readonly string _connectionString;

        private ApiFactory()
        {
            // Start the container
            DbContainer.StartAsync().GetAwaiter().GetResult();

            DbContainer.ExecScriptAsync("CREATE DATABASE test").GetAwaiter().GetResult();

            var hostPort = DbContainer.GetMappedPublicPort(1433);
            _connectionString = $"Server=localhost,{hostPort};Database=test;User Id=sa;Password=zhskalComplexPass12;";
        }

        public static ApiFactory GetInstance()
        {
            if (_instance is null)
            {
                _instance = new ApiFactory();
            }

            return _instance;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Override configuration of app
                configBuilder.Sources.Clear();
                configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:DefaultConnection", _connectionString},
                    {"Jwt:Key", "636457aca8cbebba01fb52fb7a60377d7cc53dea"},
                    {"Jwt:Issuer", "AuctionsWebApp"},
                    {"Jwt:Audience", "MyWebApp"},
                    {"AllowedHosts", "*"}
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuctionContext));
                services.AddDbContext<AuctionContext>(options =>
                    options.UseSqlServer(_connectionString));

                services.AddScoped<IAuctionContext>(provider => provider.GetService<AuctionContext>()!);

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AuctionContext>();

                    // Ensure the database is created and migrations are applied
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}