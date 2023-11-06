using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using webapi.Database;


namespace Api.IntegrationTests
{
    public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuctionContext));
                var connectionString = $"Server=localhost;Database=test;User Id=sa;Password=zhskalComplexPass12;";
                services.AddDbContext<AuctionContext>(options =>
                    options.UseSqlServer(connectionString));

                services.AddScoped<IAuctionContext>(provider => provider.GetService<AuctionContext>()!);
            });
        }


        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}
