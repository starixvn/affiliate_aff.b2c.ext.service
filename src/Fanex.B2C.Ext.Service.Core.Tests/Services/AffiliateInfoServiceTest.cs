using System.Data.Common;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class AffiliateInfoServiceTest
    {
        public AffiliateInfoServiceTest()
        {
            DbSettingProviderManager
             .StartTestingSession()
             .Run();
        }

        [Fact]
        public void GetAffInfo_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var param = new AffInfoParam();
            var mockObjectDB = Substitute.For<IObjectDb>();
            mockObjectDB.QueryEntity<AffInfo>(Arg.Any<object>(), Arg.Any<DbTransaction>()).Returns(new AffInfo());

            ObjectDbFactory.SwitchToTestingMode();
            ObjectDbFactory.Mock((x, y) => mockObjectDB);
            var service = new AffililateInfoService();

            //Action
            var result = service.GetAffInfo(param);

            //Assert
            Assert.NotNull(JsonConvert.SerializeObject(result));
            mockObjectDB.Received().SetParameterValue("@MonthDate", param.MonthDate);
            mockObjectDB.Received().SetParameterValue("@Player_Username", param.PlayerName);
            mockObjectDB.Received().SetParameterValue("@BaseCurrencyName", param.BaseCurrencyName);
        }

        [Fact]
        public void GetAffDetailInfoList_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var param = new AffDetailInfoParam();
            var mockObjectDB = Substitute.For<IObjectDb>();
            mockObjectDB.Query<AffDetailInfo>(Arg.Any<object>(), Arg.Any<DbTransaction>()).Returns(new List<AffDetailInfo>{ new AffDetailInfo() });

            ObjectDbFactory.SwitchToTestingMode();
            ObjectDbFactory.Mock((x, y) => mockObjectDB);
            var service = new AffililateInfoService();

            //Action
            var result = service.GetAffDetailInfoList(param);

            //Assert
            Assert.NotNull(JsonConvert.SerializeObject(result));
            mockObjectDB.Received().Query<AffDetailInfo>(param);
        }
    }
}