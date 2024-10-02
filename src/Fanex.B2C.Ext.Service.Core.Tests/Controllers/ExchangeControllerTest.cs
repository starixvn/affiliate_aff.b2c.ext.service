using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
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
            var result = await controller.Post(new DailyExchangeRateCriteria()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("USD ")]
        public async Task Post_WhenCall_CallLogInformationAndReturnCorrect(string exchangeIds)
        {
            //Arrange
            _exchangeService.GetDailyExchangeRateAsync(Arg.Any<DailyExchangeRateCriteria>()).Returns(new List<DailyExchangeRate>{new DailyExchangeRate {
                ExchangeId = 123,
                Code = "USD",
                Currency = "USD",
                Rate = 1,
            }});
            var controller = new ExchangeController(_logger, _exchangeService);

            //Action
            var result = await controller.Post(new DailyExchangeRateCriteria { ExchangeIds = exchangeIds }) as ObjectResult;

            //Assert
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Rate");
            Assert.Single(result?.Value as List<DailyExchangeRate>);
            Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);
        }
    }
}