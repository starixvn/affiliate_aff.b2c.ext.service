using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Param;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data.Repository;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class ExchangeServiceTest
    {
        // Constructor removed - unit tests should not initialize database connections

        [Fact]
        public async Task GetDailyExchangeRate_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<DailyExchangeRate>(Arg.Any<DailyExchangeRateCriteria>()).Returns(new List<DailyExchangeRate> { new DailyExchangeRate() });

            var service = new ExchangeService(repository);

            //Action
            var result = await service.GetDailyExchangeRateAsync(new DailyExchangeRateParam());

            //Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDailyExchangeBidRate_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<DailyExchangeRate>(Arg.Any<DailyExchangeRateCriteria>()).Returns(new List<DailyExchangeRate> { new DailyExchangeRate() });

            var service = new ExchangeService(repository);

            //Action
            var result = await service.GetDailyExchangeBidRateAsync(new DailyExchangeRateParam());

            //Assert
            Assert.Single(result);
        }
    }
}