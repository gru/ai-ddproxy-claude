using System.Net;
using AI.DaDataProxy.DaData;
using AI.DaDataProxy.Http.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Text.Json;
using RestEase;
using Xunit;
using System.Net.Http.Json;

namespace AI.DaDataProxy.Tests
{
    public class DaDataHandlerCachingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IDaDataController _client;
        private readonly IRedisCache _mockRedisCache;
        private readonly IDaDataApi _mockDaDataApi;

        public DaDataHandlerCachingTests(WebApplicationFactory<Program> factory)
        {
            _mockRedisCache = Substitute.For<IRedisCache>();
            _mockDaDataApi = Substitute.For<IDaDataApi>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockRedisCache);
                    services.AddSingleton(_mockDaDataApi);
                    services.PostConfigure<DaDataCachingOptions>(options =>
                    {
                        options.DailyRequestLimit = 1000;
                        options.RequestCounterExpirationHours = 24;
                        options.DefaultCacheDurationInHours = 1;
                    });
                });
            });

            var client = _factory.CreateClient();
            _client = RestClient.For<IDaDataController>(client);
        }

        [Fact]
        public async Task ProxyRequest_CachedResult_ReturnsCachedData()
        {
            // Arrange
            var path = "test/path";
            var body = new { query = "test body" };
            var cachedResult = "cached result";

            _mockRedisCache.GetCachedQueryAsync(Arg.Any<string>()).Returns(cachedResult);

            // Act
            using var content = JsonContent.Create(body);
            var result = await _client.ProxyRequest(path, content);

            // Assert
            Assert.Equal(cachedResult, result);
        }

        [Fact]
        public async Task ProxyRequest_NoCachedResult_CallsApiAndCachesResult()
        {
            // Arrange
            var path = "test/path";
            var body = new { query = "test body" };

            _mockDaDataApi.ProxyRequestAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(JsonSerializer.Serialize(new { response = "test response" }));
            _mockRedisCache.GetCachedQueryAsync(Arg.Any<string>()).Returns(default(string));

            // Act
            using var content = JsonContent.Create(body);
            var result = await _client.ProxyRequest(path, content);

            // Assert
            Assert.NotNull(result);
            await _mockRedisCache.Received(1).SetCachedQueryAsync(
                Arg.Any<string>(),
                Arg.Is<string>(v => !string.IsNullOrEmpty(v)),
                Arg.Any<TimeSpan>());
        }

        [Fact]
        public async Task ProxyRequest_DailyLimitExceeded_ThrowsDaDataTooManyRequests()
        {
            // Arrange
            var path = "test/path";
            var body = new { query = "test body" };

            _mockRedisCache.GetDailyRequestCountAsync(Arg.Any<DateTime>()).Returns(1000);

            // Act & Assert
            using var content = JsonContent.Create(body);
            var ex = await Assert.ThrowsAsync<ApiException>(() => _client.ProxyRequest(path, content));
            Assert.Equal(HttpStatusCode.TooManyRequests, ex.StatusCode);
        }

        [Fact]
        public async Task ProxyRequest_LegalEntityByInnRequest_UsesCorrectCacheExpiration()
        {
            // Arrange
            var path = "findById/party";
            var body = new { query = "7707083893" };

            _mockDaDataApi.ProxyRequestAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(JsonSerializer.Serialize(new { query = "7707083893" }));
            _mockRedisCache.GetCachedQueryAsync(Arg.Any<string>()).Returns(default(string));

            // Act
            using var content = JsonContent.Create(body);
            await _client.ProxyRequest(path, content);

            // Assert
            await _mockRedisCache.Received(1).SetCachedQueryAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is<TimeSpan>(t => t.Days == 30)); // Assuming LegalEntityCacheDurationInDays is 30
        }
    }
}
