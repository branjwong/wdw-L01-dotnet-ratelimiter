using Microsoft.AspNetCore.Mvc.Testing;
using SimpleRateLimiter;
using System.Net.Http.Json;

namespace SimpleRateLimiter.Tests.IntegrationTests;

public class TakeTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TakeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Theory]
    [InlineData("/asdf")]
    [InlineData("/not/exist")]
    public async Task Take_Returns404_IfRouteDoesNotExist(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/take")]
    public async Task Take_Returns400_IfMissingEndpointBodyParam(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(url, new { });

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/take")]
    public async Task Take_Returns400_IfClientWaitsUntilTokensAvailable(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        for (int i = 0; i < 310; i++)
        {
            await client.PostAsJsonAsync(url, new { Endpoint = "POST /userinfo" });
        }

        await Task.Run(() => Thread.Sleep(1000));

        var response = await client.PostAsJsonAsync(url, new { Endpoint = "POST /userinfo" });

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
