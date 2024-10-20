using AI.DaDataProxy.Entities;
using AI.DaDataProxy.Http.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Xunit;

namespace AI.DaDataProxy.Tests;

public class DaDataIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IDaDataV1Controller _client;

    public DaDataIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DaDataProxyDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContextPool<DaDataProxyDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            });
        });

        var httpClient = testFactory.CreateClient();
        _client = RestClient.For<IDaDataV1Controller>(httpClient);
    }

    [Fact]
    public async Task CreateAndGetDaData_ShouldSucceed()
    {
        var command = new CreateDaDataCommand
        {
            Name = "Test DaData"
        };

        var id = await _client.CreateDaData(command);
        var aggregate = await _client.GetDaData(id);

        Assert.NotEqual(0, id);
        Assert.Equal(id, aggregate.Id);
        Assert.Equal(command.Name, aggregate.Name);
    }
}