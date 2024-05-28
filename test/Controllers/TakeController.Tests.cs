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
        [Fact]
        public async Task Take_Returns400IfRouteMissing()
        {
            // Arrange
            var controller = new Setup().CreateController();

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = "" });

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task Take_Returns400IfRouteNotFound()
        {
            // Arrange
            var controller = new Setup().CreateController();

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = "GET bad/request/url" });

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task Take_Returns200IfTokensAvailable()
        {
            // Arrange
            var controller = new Setup().CreateController();

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = "GET /user/:id" });

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

        [Fact]
        public async Task Take_Returns400IfClientWaitsUntilTokensAvailable()
        {
            // Arrange
            var controller = new Setup().CreateController();

            for (int i = 0; i < 300; i++)
            {
                await controller.PostEndpointBucket(new TakeItem { Endpoint = "POST /userinfo" });
            }

            await Task.Run(() => Thread.Sleep(300));

            // Act
            var response = await controller.PostEndpointBucket(new TakeItem { Endpoint = "POST /userinfo" });

            // Assert
            Assert.Equal(200, response.StatusCode);
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
