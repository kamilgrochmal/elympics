using ElympicsNet.Api.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace ElympicsNet.Api.Tests.Integration;

public sealed class ElympicsNetApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer
        = new PostgreSqlBuilder()
            .WithDatabase("test-db")
            .WithUsername("test-user")
            .WithPassword("testpassword")
            .Build();

    private readonly GoWebApiServer _goWebApiServer = new ();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logger =>
            logger.ClearProviders());

        builder.ConfigureTestServices(services =>
        {

            services.RemoveAll(typeof(ApplicationDbContext));
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            
            services.AddDbContext<ApplicationDbContext>(a => a.UseNpgsql(_dbContainer.GetConnectionString()));

            services.AddHttpClient(_goWebApiServer.GoWebApiSectionName, httpClient =>
            {
                httpClient.BaseAddress = new Uri(_goWebApiServer.Url);
            });
        });
    }

    public async Task InitializeAsync()
    {
        _goWebApiServer.Start();
        _goWebApiServer.SetupPodcast(title:"Musisz wiedzieć - WireMock", hostedBy: "N/A");
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _goWebApiServer.Dispose();
    }
}