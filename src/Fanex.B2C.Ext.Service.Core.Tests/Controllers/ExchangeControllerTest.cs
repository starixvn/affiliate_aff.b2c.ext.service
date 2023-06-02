using System.Net;
using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

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
        public void Post_WhenCallWith__InvalidModel_ReturnBadRequest()
        {
            //Arrange
            var controller = new ExchangeController(_logger, _exchangeService);
            controller.ModelState.AddModelError("Error1", "Invalid");

            //Action
            var result = controller.Post(new DailyExchangeRateParam()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("USD ")]
        public void Post_WhenCall_CallLogInformationAndReturnCorrect(string exchangeIds)
        {
            //Arrange
            var expectedParam = new DailyExchangeRateParam { ExchangeIds = "USD", ExchangeDate = new DateTime(2023, 05, 30) };
            _exchangeService.GetDailyExchangeRate(Arg.Any<DailyExchangeRateParam>()).Returns(new List<DailyExchangeRate>{new DailyExchangeRate {
                ExchangeId = 123,
                Code = "USD",
                Currency = "USD",
                Rate = 1,
            }});
            var controller = new ExchangeController(_logger, _exchangeService);

            //Action
            var result = controller.Post(new DailyExchangeRateParam { ExchangeIds = exchangeIds }) as ObjectResult;

            //Assert
            _logger.Received().LogInformation("Ext B2C - Get Daily Exchange Rate");
            Assert.Single(result?.Value as List<DailyExchangeRate>);
            Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);
        }
    }
}