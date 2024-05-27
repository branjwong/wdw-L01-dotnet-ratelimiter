using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SimpleRateLimiter.Controllers;
using SimpleRateLimiter.Models;
using Microsoft.Extensions.Logging;

namespace SimpleRateLimiter.Tests.UnitTests
{
    public class TakeControllerTests
    {
        [Fact]
        public void Take_Returns400IfRouteMissing()
        {
            // Arrange
            var mock = new Mock<ILogger<TakeController>>();
            ILogger<TakeController> logger = mock.Object;

            var controller = new TakeController(logger);

            // Act
            var response = controller.Index(new TakeItem());

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public void Take_Returns400IfRouteNotFound()
        {
            // Arrange
            var mock = new Mock<ILogger<TakeController>>();
            ILogger<TakeController> logger = mock.Object;

            var controller = new TakeController(logger);

            // Act
            var response = controller.Index(new TakeItem { Endpoint = "GET bad/request/url" });

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public void Take_Returns200IfTokensAvailable()
        {
            // Arrange
            var mock = new Mock<ILogger<TakeController>>();
            ILogger<TakeController> logger = mock.Object;

            var controller = new TakeController(logger);

            // Act
            var response = controller.Index(new TakeItem { Endpoint = "GET /user/:id" });

            // Assert
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public void Take_Returns429IfNoTokensAvailable()
        {
            // Arrange
            var mock = new Mock<ILogger<TakeController>>();
            ILogger<TakeController> logger = mock.Object;

            var controller = new TakeController(logger);

            for (int i = 0; i < 10; i++)
            {
                controller.Index(new TakeItem { Endpoint = "GET /user/:id" });
            }

            // Act
            var response = controller.Index(new TakeItem { Endpoint = "GET /user/:id" });

            // Assert
            Assert.Equal(429, response.StatusCode);
        }

        [Fact]
        public async Task Take_Returns400IfClientWaitsUntilTokensAvailable()
        {
            // Arrange
            var mock = new Mock<ILogger<TakeController>>();
            ILogger<TakeController> logger = mock.Object;

            var controller = new TakeController(logger);

            for (int i = 0; i < 300; i++)
            {
                controller.Index(new TakeItem { Endpoint = "POST /userinfo" });
            }

            await Task.Run(() => Thread.Sleep(300));

            // Act
            var response = controller.Index(new TakeItem { Endpoint = "POST /userinfo" });

            // Assert
            Assert.Equal(200, response.StatusCode);
        }
    }
}
