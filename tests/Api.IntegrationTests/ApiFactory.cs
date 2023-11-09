using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuctionContext));

                _dbContainer.StartAsync().GetAwaiter().GetResult();

                //var connectionString = _dbContainer.GetConnectionString();

                var hostPort = _dbContainer.GetMappedPublicPort(1433);
                var connectionString = $"Server=localhost,{hostPort};Database=test;User Id=sa;Password=zhskalComplexPass12;";


                services.AddDbContext<AuctionContext>(options =>
                    options.UseSqlServer(connectionString));

                services.AddScoped<IAuctionContext>(provider => provider.GetService<AuctionContext>()!);
            });
        }

    }
}
