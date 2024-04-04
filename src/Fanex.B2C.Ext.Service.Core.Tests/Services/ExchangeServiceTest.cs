using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data;
using NSubstitute;
using System.Data.Common;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class ExchangeServiceTest
    {
        public ExchangeServiceTest()
        {
            DbSettingProviderManager
             .StartTestingSession()
             .Run();
        }

        [Fact]
        public void GetDailyExchangeRate_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var param = new DailyExchangeRateParam();
            var mockObjectDB = Substitute.For<IObjectDb>();
            mockObjectDB.Query<DailyExchangeRate>(Arg.Any<object>(), Arg.Any<DbTransaction>()).Returns(new List<DailyExchangeRate> { new DailyExchangeRate() });

            ObjectDbFactory.SwitchToTestingMode();
            ObjectDbFactory.Mock((x, y) => mockObjectDB);
            var service = new ExchangeService();

            //Action
            var result = service.GetDailyExchangeRate(param);

            //Assert
            Assert.Single(result);
            mockObjectDB.Received().Query<DailyExchangeRate>(param);
        }
    }
}