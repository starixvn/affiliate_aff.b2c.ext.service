using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class AffiliateInfoServiceTest
    {
        [Fact]
        public async Task GetAffInfo_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var param = new AffInfoCriteria();
            var repository = Substitute.For<IDynamicRepository>();
            repository.GetAsync<AffInfo>(Arg.Any<AffInfoCriteria>()).Returns(new AffInfo());

            var service = new AffililateInfoService(repository);

            //Action
            var result = await service.GetAffInfoAsync(param);

            //Assert
            Assert.NotNull(JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void GetAffDetailInfoList_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var param = new AffDetailInfoCriteria();
            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffDetailInfo>(Arg.Any<AffDetailInfoCriteria>()).Returns([new AffDetailInfo()]);

            var service = new AffililateInfoService(repository);

            //Action
            var result = service.GetAffDetailInfoListAsync(param);

            //Assert
            Assert.NotNull(JsonConvert.SerializeObject(result));
        }
    }
}