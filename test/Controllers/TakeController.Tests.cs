using Moq;
using SimpleRateLimiter.Controllers;
using SimpleRateLimiter.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SimpleRateLimiter.Services;

namespace SimpleRateLimiter.Tests.UnitTests
{
    public class TakeControllerTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("GET bad/request/url")]
        [InlineData("GET not/exist/url")]
        public async Task Take_Returns400IfRouteNotFound(string endpoint)
        {
            // Arrange
            var controller = new Setup().CreateController();

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = endpoint });

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        [Theory]
        [InlineData("GET /user/:id")]
        [InlineData("POST /userinfo")]
        public async Task Take_Returns200IfTokensAvailable(string endpoint)
        {
            // Arrange
            var controller = new Setup().CreateController();

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = endpoint });

            // Assert
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task Take_Returns429IfNoTokensAvailable()
        {
            // Arrange
            var controller = new Setup().CreateController();

            for (int i = 0; i < 10; i++)
            {
                await controller.PostEndpointBucket(new TakeItem { Endpoint = "GET /user/:id" });
            }

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = "GET /user/:id" });

            // Assert
            Assert.Equal(429, response.StatusCode);
        }
    }

    public class Setup
    {
        public IBucketManager bucketManager;
        public ILogger<TakeController> logger;

        public Setup()
        {
            var mock = new Mock<ILogger<TakeController>>();
            logger = mock.Object;

            var options = new DbContextOptionsBuilder<BucketContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            var context = new BucketContext(options);
            context.Database.EnsureDeleted();
            context.EndpointBuckets.Add(new EndpointBucket { Endpoint = "GET /user/:id", Tokens = 10 });
            context.EndpointBuckets.Add(new EndpointBucket { Endpoint = "POST /userinfo", Tokens = 1 });
            context.SaveChanges();

            bucketManager = new BucketManager(context);
        }

        public TakeController CreateController()
        {
            return new TakeController(logger, bucketManager);
        }
    }
}
