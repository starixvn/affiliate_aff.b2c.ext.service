using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Param;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class ExchangeControllerTest
    {
        private readonly ILogger<ExchangeController> _logger;
        private readonly IExchangeService _exchangeService;

        public ExchangeControllerTest()
        {
            _logger = Substitute.For<ILogger<ExchangeController>>();
            _exchangeService = Substitute.For<IExchangeService>();
        }

        [Fact]
        public async Task Post_WhenCallWith__InvalidModel_ReturnBadRequest()
        {
            //Arrange
            var controller = new ExchangeController(_logger, _exchangeService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = await controller.Post(new DailyExchangeRateParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("USD ")]
        public async Task Post_WhenCall_CallLogInformationAndReturnCorrect(string exchangeIds)
        {
            //Arrange
            _exchangeService.GetDailyExchangeRateAsync(Arg.Any<DailyExchangeRateParam>()).Returns(new List<DailyExchangeRate>{new DailyExchangeRate {
                ExchangeId = 123,
                Code = "USD",
                Currency = "USD",
                Rate = 1,
            }});
            var controller = new ExchangeController(_logger, _exchangeService);

            //Action
            var result = await controller.Post(new DailyExchangeRateParam { ExchangeIds = exchangeIds }) as ObjectResult;

            //Assert
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Rate");
            Assert.Single(result?.Value as List<DailyExchangeRate>);
            Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenValidDataProvided()
        {
            // Arrange - Valid exchange rate request scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "USD,EUR,GBP"
            };
            var expectedRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m },
                new DailyExchangeRate { ExchangeId = 2, Code = "EUR", Currency = "EUR", Rate = 0.85m }
            };

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(expectedRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get exchange rates
            var result = await controller.Post(param);

            // Assert - Should return Ok with exchange rates
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(expectedRates);
        }

        [Fact]
        public async Task Post_ShouldSanitizeExchangeIds_WhenProcessingRequest()
        {
            // Arrange - Exchange IDs sanitization scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "  <script>alert('xss')</script>USD,EUR  "
            };
            var expectedRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m }
            };

            _exchangeService.GetDailyExchangeRateAsync(Arg.Any<DailyExchangeRateParam>()).Returns(expectedRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Process request with potentially malicious input
            var result = await controller.Post(param);

            // Assert - Should sanitize input and return Ok
            result.Should().BeOfType<OkObjectResult>();

            // Verify that ExchangeIds was sanitized (HTML encoded and trimmed)
            param.ExchangeIds.Should().NotContain("<script>");
            param.ExchangeIds.Should().NotContain("</script>");
            // Check that dangerous HTML is encoded
            param.ExchangeIds.Should().Contain("&lt;script&gt;"); // HTML encoded script tag
        }

        [Fact]
        public async Task Post_ShouldSupportBusinessScenario_B2CExchangeRateRetrieval()
        {
            // Arrange - B2C exchange rate retrieval business scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "USD,EUR,GBP,JPY,SGD"
            };
            var b2cExchangeRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m },
                new DailyExchangeRate { ExchangeId = 2, Code = "EUR", Currency = "EUR", Rate = 0.85m },
                new DailyExchangeRate { ExchangeId = 3, Code = "GBP", Currency = "GBP", Rate = 0.73m },
                new DailyExchangeRate { ExchangeId = 4, Code = "JPY", Currency = "JPY", Rate = 110.0m },
                new DailyExchangeRate { ExchangeId = 5, Code = "SGD", Currency = "SGD", Rate = 1.35m }
            };

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(b2cExchangeRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get B2C exchange rates
            var result = await controller.Post(param);

            // Assert - Should return B2C exchange rates
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(b2cExchangeRates);

            // Verify service interaction
            await _exchangeService.Received(1).GetDailyExchangeRateAsync(param);
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Rate");
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("EUR,GBP")]
        [InlineData("USD,EUR,GBP,JPY")]
        public async Task Post_ShouldHandleVariousExchangeIdFormats_Correctly(string exchangeIds)
        {
            // Arrange - Various exchange ID formats scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = exchangeIds
            };
            var exchangeRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m }
            };

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(exchangeRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get exchange rates for different formats
            var result = await controller.Post(param);

            // Assert - Should handle all formats correctly
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(exchangeRates);

            // Verify ExchangeIds was processed
            param.ExchangeIds.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Post_ShouldSupportBusinessScenario_RealTimeExchangeRateUpdates()
        {
            // Arrange - Real-time exchange rate updates business scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "USD,EUR"
            };
            var realtimeRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m },
                new DailyExchangeRate { ExchangeId = 2, Code = "EUR", Currency = "EUR", Rate = 0.85m }
            };

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(realtimeRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get real-time exchange rates
            var result = await controller.Post(param);

            // Assert - Should return current exchange rates
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(realtimeRates);

            // Verify logging for audit trail
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Rate");
        }

        [Fact]
        public async Task Post_ShouldSupportBusinessScenario_MultiCurrencySupport()
        {
            // Arrange - Multi-currency support business scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "USD,EUR,GBP,JPY,SGD,HKD,CNY,THB,MYR,IDR"
            };
            var multiCurrencyRates = new List<DailyExchangeRate>();

            // Add rates for all supported currencies
            var currencies = new[] { "USD", "EUR", "GBP", "JPY", "SGD", "HKD", "CNY", "THB", "MYR", "IDR" };
            for (int i = 0; i < currencies.Length; i++)
            {
                multiCurrencyRates.Add(new DailyExchangeRate
                {
                    ExchangeId = i + 1,
                    Code = currencies[i],
                    Currency = currencies[i],
                    Rate = 1.0m + (i * 0.1m)
                });
            }

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(multiCurrencyRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get multi-currency exchange rates
            var result = await controller.Post(param);

            // Assert - Should return all currency rates
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(multiCurrencyRates);

            var returnedRates = okResult.Value as List<DailyExchangeRate>;
            returnedRates.Should().HaveCount(10);
            returnedRates.Should().OnlyContain(rate => rate.Rate > 0);
        }

        [Fact]
        public async Task Post_ShouldSupportBusinessScenario_SecurityInputValidation()
        {
            // Arrange - Security input validation business scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "<img src=x onerror=alert('XSS')>USD</img>"
            };
            var safeRates = new List<DailyExchangeRate>
            {
                new DailyExchangeRate { ExchangeId = 1, Code = "USD", Currency = "USD", Rate = 1.0m }
            };

            _exchangeService.GetDailyExchangeRateAsync(Arg.Any<DailyExchangeRateParam>()).Returns(safeRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Process potentially malicious input
            var result = await controller.Post(param);

            // Assert - Should sanitize input and process safely
            result.Should().BeOfType<OkObjectResult>();

            // Verify input was sanitized (HTML encoded)
            param.ExchangeIds.Should().NotContain("<img");
            param.ExchangeIds.Should().NotContain("</img>");
            // Check that dangerous HTML is encoded
            param.ExchangeIds.Should().Contain("&lt;img"); // HTML encoded img tag
            param.ExchangeIds.Should().Contain("&gt;"); // HTML encoded closing bracket
        }

        [Fact]
        public async Task Post_ShouldSupportBusinessScenario_EmptyExchangeRateResponse()
        {
            // Arrange - Empty exchange rate response business scenario
            var param = new DailyExchangeRateParam
            {
                ExchangeIds = "INVALID_CURRENCY"
            };
            var emptyRates = new List<DailyExchangeRate>();

            _exchangeService.GetDailyExchangeRateAsync(param).Returns(emptyRates);
            var controller = new ExchangeController(_logger, _exchangeService);

            // Act - Get exchange rates for invalid currency
            var result = await controller.Post(param);

            // Assert - Should return Ok with empty list
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(emptyRates);

            var returnedRates = okResult.Value as List<DailyExchangeRate>;
            returnedRates.Should().BeEmpty();
        }

        [Fact]
        public async Task PostBidRate_WhenCallWith__InvalidModel_ReturnBadRequest()
        {
            //Arrange
            var controller = new ExchangeController(_logger, _exchangeService);
            controller.ModelState.AddModelError("Error1", "Bid Rate Invalid");

            //Action
            var result = await controller.PostBidRate(new DailyExchangeRateParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("USD ")]
        public async Task PostBidRate_WhenCall_CallLogInformationAndReturnCorrect(string exchangeIds)
        {
            //Arrange
            _exchangeService.GetDailyExchangeBidRateAsync(Arg.Any<DailyExchangeRateParam>()).Returns(new List<DailyExchangeRate>{new DailyExchangeRate {
                ExchangeId = 123,
                Code = "USD",
                Currency = "USD",
                Rate = 1,
            }});
            var controller = new ExchangeController(_logger, _exchangeService);

            //Action
            var result = await controller.PostBidRate(new DailyExchangeRateParam { ExchangeIds = exchangeIds }) as ObjectResult;

            //Assert
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Bid Rate");
            Assert.Single(result?.Value as List<DailyExchangeRate>);
            Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);
        }
    }
}