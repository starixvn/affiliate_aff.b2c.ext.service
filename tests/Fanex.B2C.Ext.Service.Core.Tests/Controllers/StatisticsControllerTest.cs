using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class StatisticsControllerTest
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly IAuthorizationService _authorizationService;

        public StatisticsControllerTest()
        {
            _logger = Substitute.For<ILogger<StatisticsController>>();
            _statisticsService = Substitute.For<IStatisticsService>();
            _authorizationService = Substitute.For<IAuthorizationService>();
        }

        #region GetAffMediaChannelAsync Tests

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenCall_CallLogInformationAndReturnCorrect()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };
            var expectedResult = new List<AffMediaChannel>
            {
                new AffMediaChannel { ChannelCode = "CH001", ChannelName = "Test Channel 1" },
                new AffMediaChannel { ChannelCode = "CH002", ChannelName = "Test Channel 2" }
            };

            _statisticsService.GetAffMediaChannelAsync(Arg.Any<AffMediaChannelCriteria>()).Returns(expectedResult);

            // Mock successful authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetAffMediaChannelAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
            _logger.Received().LogInformation($"Ext B2C - Get aff {criteria.LicenseeCode} media channel");
        }

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("Error", "Invalid model");

            // Action
            var result = await controller.GetAffMediaChannelAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenAuthorizationFails_ReturnForbidden()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };

            // Mock failed authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Failed());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetAffMediaChannelAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal("The licensee is either invalid or not authorized to use this function!", result.Value);
        }

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenWrongLicenseeCode_ReturnForbidden()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "N8B" }; // Wrong licensee code

            // Mock successful authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetAffMediaChannelAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal("The licensee is either invalid or not authorized to use this function!", result.Value);
        }

        #endregion

        #region GetClickStatisticAsync Tests

        [Fact]
        public async Task GetClickStatisticAsync_WhenCall_CallLogInformationAndReturnCorrect()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria
            {
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = 1,
                PageSize = 100
            };

            var expectedResult = new List<ClickStatistic>
            {
                new ClickStatistic
                {
                    ClickId = 1001,
                    AffId = 123,
                    AffCode = "AFF001",
                    AffUserName = "TestUser",
                    IP = "192.168.1.1",
                    Browser = "Chrome",
                    Country = "US"
                }
            };

            _statisticsService.GetClickStatisticAsync(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            // Mock successful authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetClickStatisticAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
            _logger.Received().LogInformation($"Ext B2C - Get click statistics from {criteria.FromDate} to {criteria.ToDate}");
        }

        [Fact]
        public async Task GetClickStatisticAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria
            {
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("Error", "Invalid model");

            // Action
            var result = await controller.GetClickStatisticAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetClickStatisticAsync_WhenAuthorizationFails_ReturnForbidden()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria
            {
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            // Mock failed authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Failed());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetClickStatisticAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal("The licensee is either invalid or not authorized to use this function!", result.Value);
        }

        [Fact]
        public async Task GetClickStatisticAsync_WhenWrongLicenseeCode_ReturnForbidden()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria
            {
                LicenseeCode = "N8B", // Wrong licensee code
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            // Mock successful authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetClickStatisticAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal("The licensee is either invalid or not authorized to use this function!", result.Value);
        }

        [Theory]
        [InlineData(1, 500)]
        [InlineData(10, 1000)]
        [InlineData(100, 100)]
        public async Task GetClickStatisticAsync_WithValidPagination_ReturnCorrect(int pageIndex, int pageSize)
        {
            // Arrange
            var criteria = new ClickStatisticCriteria
            {
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var expectedResult = new List<ClickStatistic>();
            _statisticsService.GetClickStatisticAsync(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            // Mock successful authorization
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(),
                Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Action
            var result = await controller.GetClickStatisticAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }

        #endregion

        #region OH Statistics Endpoints Tests

        [Fact]
        public async Task GetOHCampaignAsync_WhenCall_ReturnOkWithData()
        {
            // Arrange
            var criteria = new OHCampaignCriteria
            {
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = 1,
                PageSize = 100
            };

            var expectedResult = new List<OHCampaign>
            {
                new OHCampaign { CampaignId = 1, CampaignName = "Test Campaign", CampaignCode = "TC001", StatusId = 1 }
            };

            _statisticsService.GetOHCampaignAsync(Arg.Any<OHCampaignCriteria>()).Returns(expectedResult);
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Act
            var result = await controller.GetOHCampaignAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }

        [Fact]
        public async Task GetOHCampaignAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new OHCampaignCriteria();
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("FromDate", "FromDate is required");

            // Act
            var result = await controller.GetOHCampaignAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetOHComponentAsync_WhenCall_ReturnOkWithData()
        {
            // Arrange
            var criteria = new OHComponentCriteria
            {
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = 1,
                PageSize = 100
            };

            var expectedResult = new List<OHComponent>
            {
                new OHComponent { ComponentId = 1, ComponentName = "Test Component", ComponentCode = "COMP001", StatusId = 1 }
            };

            _statisticsService.GetOHComponentAsync(Arg.Any<OHComponentCriteria>()).Returns(expectedResult);
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Act
            var result = await controller.GetOHComponentAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }

        [Fact]
        public async Task GetOHComponentAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new OHComponentCriteria();
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("FromDate", "FromDate is required");

            // Act
            var result = await controller.GetOHComponentAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetOHLinkGeneratorAsync_WhenCall_ReturnOkWithData()
        {
            // Arrange
            var criteria = new OHLinkGeneratorCriteria
            {
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = 1,
                PageSize = 100
            };

            var expectedResult = new List<OHLinkGenerator>
            {
                new OHLinkGenerator { LinkId = 1, LinkName = "Test Link", IDCode = "LNK001", FullLink = "https://example.com/link1", StatusId = 1 }
            };

            _statisticsService.GetOHLinkGeneratorAsync(Arg.Any<OHLinkGeneratorCriteria>()).Returns(expectedResult);
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Act
            var result = await controller.GetOHLinkGeneratorAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }

        [Fact]
        public async Task GetOHLinkGeneratorAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new OHLinkGeneratorCriteria();
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("FromDate", "FromDate is required");

            // Act
            var result = await controller.GetOHLinkGeneratorAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task GetOHClickStatisticsAsync_WhenCall_ReturnOkWithData()
        {
            // Arrange
            var criteria = new OHClickStatisticsCriteria
            {
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = 1,
                PageSize = 100
            };

            var expectedResult = new List<OHClickStatistics>
            {
                new OHClickStatistics
                {
                    ClickId = 1001,
                    IP = "192.168.1.1",
                    Browser = "Chrome",
                    LinkId = 1,
                    LinkName = "Test Link",
                    IDCode = "LNK001"
                }
            };

            _statisticsService.GetOHClickStatisticsAsync(Arg.Any<OHClickStatisticsCriteria>()).Returns(expectedResult);
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Act
            var result = await controller.GetOHClickStatisticsAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }

        [Fact]
        public async Task GetOHClickStatisticsAsync_WhenInvalidModel_ReturnBadRequest()
        {
            // Arrange
            var criteria = new OHClickStatisticsCriteria();
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);
            controller.ModelState.AddModelError("FromDate", "FromDate is required");

            // Act
            var result = await controller.GetOHClickStatisticsAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(10, 500)]
        [InlineData(1, 1000)]
        public async Task GetOHCampaignAsync_WithValidPagination_ReturnCorrect(int pageIndex, int pageSize)
        {
            // Arrange
            var criteria = new OHCampaignCriteria
            {
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var expectedResult = new List<OHCampaign>();
            _statisticsService.GetOHCampaignAsync(Arg.Any<OHCampaignCriteria>()).Returns(expectedResult);
            var controller = new StatisticsController(_logger, _statisticsService, _authorizationService);

            // Act
            var result = await controller.GetOHCampaignAsync(criteria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        #endregion
    }
}