using Api.IntegrationTests.ItemController;
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
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithPassword("zhskalComplexPass12")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithExposedPort(1433)
            .Build();

        private readonly string _connectionString;

        public ApiFactory()
        {
            // Start the container
            _dbContainer.StartAsync().GetAwaiter().GetResult();

            _dbContainer.ExecScriptAsync("CREATE DATABASE test").GetAwaiter().GetResult();

            var hostPort = _dbContainer.GetMappedPublicPort(1433);
            _connectionString = $"Server=localhost,{hostPort};Database=test;User Id=sa;Password=zhskalComplexPass12;";

            Environment.SetEnvironmentVariable("ASPNET__isTestEnvironment", "true");
            Environment.SetEnvironmentVariable("ASPNET__TempDbConnectionString", $"{_connectionString}");
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
                    {"AllowedHosts", "*"},
                    {"FileStorage:BasePath", "C:\\ProgramData\\EbayAuctionTest\\wwwroot\\item-images\\"}
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