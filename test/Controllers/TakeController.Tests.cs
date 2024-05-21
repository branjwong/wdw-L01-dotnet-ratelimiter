using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SimpleRateLimiter.Controllers;

namespace SimpleRateLimiter.Tests.UnitTests
{
    public class TakeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsHi()
        {
            // Arrange
            var controller = new TakeController();

            // Act
            var result = await controller.Index();

            // Assert
            var stringResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<string>>(result);
            Assert.Equal("hi", stringResult.Value);
        }
    }
}
