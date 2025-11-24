using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class AffInfoControllerTest
    {
        [Fact]
        public async Task GetAffInfo_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffInfoCriteria { LicenseeCode = "N8B" };

            // Create a test user with claims
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "test"));

            // Create fresh substitutes for this test
            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            affililateInfoService.GetAffInfoAsync(Arg.Any<AffInfoCriteria>()).Returns(new AffInfo());

            // Mock the base IAuthorizationService method that the extension method calls
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
                .Returns(AuthorizationResult.Success());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);

            // Set up controller context with the test user
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            //Action
            var result = await controller.GetAffInfoAsync(param);

            //Assert
            Assert.NotNull(result);
            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");
        }

        [Fact]
        public async Task GetAffDetailInfoList_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffDetailInfoCriteria { LicenseeCode = "N88" };

            // Create a test user with claims
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "test"));

            // Create fresh substitutes for this test
            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            affililateInfoService.GetAffDetailInfoListAsync(Arg.Any<AffDetailInfoCriteria>()).Returns([new AffDetailInfo()]);

            // Mock the base IAuthorizationService method that the extension method calls
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
                .Returns(AuthorizationResult.Success());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);

            // Set up controller context with the test user
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            //Action
            var result = await controller.GetAffDetailInfoListAsync(param);

            //Assert
            Assert.NotNull(result);
            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");
        }

        [Fact]
        public async Task GetAffInfoAsync_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange - Invalid model state scenario
            var param = new AffInfoCriteria { LicenseeCode = "12B" };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Add model error
            controller.ModelState.AddModelError("PlayerName", "PlayerName is required");

            // Act - Call with invalid model state
            var result = await controller.GetAffInfoAsync(param);

            // Assert - Should return BadRequest
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAffInfoAsync_ShouldReturnForbidden_WhenAuthorizationFails()
        {
            // Arrange - Authorization failure scenario
            var param = new AffInfoCriteria { LicenseeCode = "UNAUTHORIZED" };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            // Mock failed authorization
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
                .Returns(AuthorizationResult.Failed());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Act - Call with unauthorized licensee
            var result = await controller.GetAffInfoAsync(param);

            // Assert - Should return Forbidden
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(403);
            objectResult.Value.Should().Be("The licensee is either invalid or not authorized to use this function!");
        }

        [Fact]
        public async Task GetAffDetailInfoListAsync_ShouldReturnForbidden_WhenAuthorizationFails()
        {
            // Arrange - Authorization failure for detail info scenario
            var param = new AffDetailInfoCriteria { LicenseeCode = "UNAUTHORIZED" };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            // Mock failed authorization
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
                .Returns(AuthorizationResult.Failed());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Act - Call with unauthorized licensee
            var result = await controller.GetAffDetailInfoListAsync(param);

            // Assert - Should return Forbidden
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(403);
            objectResult.Value.Should().Be("The licensee is either invalid or not authorized to use this function!");
        }

        [Fact]
        public async Task GetAffInfoAsync_ShouldSupportBusinessScenario_B2CAffiliateInfoRetrieval()
        {
            // Arrange - B2C affiliate info retrieval business scenario
            var param = new AffInfoCriteria
            {
                LicenseeCode = "12B",
                PlayerName = "b2c_player",
                BaseCurrencyName = "USD"
            };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "b2c_user")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            var b2cAffInfo = new AffInfo
            {
                AffId = "B2C_123",
                AffName = "B2C Affiliate",
                TotalPlayers = 150,
                TotalEarning = 5000.00m
            };

            affililateInfoService.GetAffInfoAsync(param).Returns(b2cAffInfo);
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Act - Retrieve B2C affiliate info
            var result = await controller.GetAffInfoAsync(param);

            // Assert - Should return B2C affiliate information
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(b2cAffInfo);

            // Verify service interaction
            await affililateInfoService.Received(1).GetAffInfoAsync(param);
            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");
        }

        [Theory]
        [InlineData("12B")]
        [InlineData("N8B")]
        [InlineData("N88")]
        public async Task GetAffInfoAsync_ShouldHandleMultipleLicenseeCodes_Correctly(string licenseeCode)
        {
            // Arrange - Multiple licensee codes scenario
            var param = new AffInfoCriteria
            {
                LicenseeCode = licenseeCode,
                PlayerName = "test_player",
                BaseCurrencyName = "USD"
            };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            var affInfo = new AffInfo { AffId = $"{licenseeCode}_001", AffName = $"{licenseeCode} Affiliate" };

            affililateInfoService.GetAffInfoAsync(param).Returns(affInfo);
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Act - Get affiliate info for different licensees
            var result = await controller.GetAffInfoAsync(param);

            // Assert - Should handle different licensee codes correctly
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(affInfo);

            // Verify authorization was checked for correct licensee
            await authorizationService.Received(1).AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                licenseeCode,
                Arg.Any<IEnumerable<IAuthorizationRequirement>>());
        }

        [Fact]
        public async Task GetAffDetailInfoListAsync_ShouldSupportBusinessScenario_AffiliatePerformanceReporting()
        {
            // Arrange - Affiliate performance reporting business scenario
            var param = new AffDetailInfoCriteria { LicenseeCode = "12B" };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "performance_user")
            }, "test"));

            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();

            var performanceData = new List<AffDetailInfo>
            {
                new AffDetailInfo
                {
                    AffCode = "TOP_PERFORMER",
                    UserName = "TopAffiliate",
                    Player_Username = "player1",
                    Status = "Active",
                    Email = "top@affiliate.com"
                },
                new AffDetailInfo
                {
                    AffCode = "MEDIUM_PERFORMER",
                    UserName = "MediumAffiliate",
                    Player_Username = "player2",
                    Status = "Active",
                    Email = "medium@affiliate.com"
                }
            };

            affililateInfoService.GetAffDetailInfoListAsync(param).Returns(performanceData);
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            // Act - Get affiliate performance data
            var result = await controller.GetAffDetailInfoListAsync(param);

            // Assert - Should return performance reporting data
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(performanceData);

            // Verify performance data structure
            var returnedData = okResult.Value as List<AffDetailInfo>;
            returnedData.Should().HaveCount(2);
            returnedData.Should().OnlyContain(aff => aff.Status == "Active");

            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");
        }
    }
}