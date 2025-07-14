using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class StatisticsServiceTest
    {
        #region GetAffMediaChannelAsync Tests

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenCall_AssignCorrectParameterAndReturnCorrectResultAsync()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };
            var expectedResult = new List<AffMediaChannel>
            {
                new AffMediaChannel { ChannelCode = "CH001", ChannelName = "Test Channel 1" },
                new AffMediaChannel { ChannelCode = "CH002", ChannelName = "Test Channel 2" }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffMediaChannel>(Arg.Any<AffMediaChannelCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetAffMediaChannelAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
            Assert.NotNull(JsonConvert.SerializeObject(result));
            await repository.Received(1).FetchAsync<AffMediaChannel>(criteria);
        }

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenRepositoryReturnsEmpty_ReturnEmptyList()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };
            var emptyResult = new List<AffMediaChannel>();

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffMediaChannel>(Arg.Any<AffMediaChannelCriteria>()).Returns(emptyResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetAffMediaChannelAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await repository.Received(1).FetchAsync<AffMediaChannel>(criteria);
        }

        [Fact]
        public async Task GetAffMediaChannelAsync_WhenRepositoryReturnsNull_ReturnNull()
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffMediaChannel>(Arg.Any<AffMediaChannelCriteria>()).Returns((IEnumerable<AffMediaChannel>)null);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetAffMediaChannelAsync(criteria);

            // Assert
            Assert.Null(result);
            await repository.Received(1).FetchAsync<AffMediaChannel>(criteria);
        }

        [Theory]
        [InlineData("12B")]
        [InlineData("N8B")]
        [InlineData("N88")]
        public async Task GetAffMediaChannelAsync_WithDifferentLicenseeCodes_CallRepositoryCorrectly(string licenseeCode)
        {
            // Arrange
            var criteria = new AffMediaChannelCriteria { LicenseeCode = licenseeCode };
            var expectedResult = new List<AffMediaChannel>
            {
                new AffMediaChannel { ChannelCode = "CH001", ChannelName = "Test Channel" }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffMediaChannel>(Arg.Any<AffMediaChannelCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetAffMediaChannelAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            await repository.Received(1).FetchAsync<AffMediaChannel>(criteria);
        }

        #endregion

        #region GetClickStatisticAsync Tests

        [Fact]
        public async Task GetClickStatisticAsync_WhenCall_AssignCorrectParameterAndReturnCorrectResultAsync()
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
                    UpdatedTime = DateTime.Now,
                    IP = "192.168.1.1",
                    Date = DateTime.Now.AddDays(-1),
                    Browser = "Chrome",
                    URLReferrer = "https://google.com",
                    DomainReferrer = "google.com",
                    OS = "Windows",
                    Device = "Desktop",
                    BrowserVersion = "91.0.4472.124",
                    BrowserResolution = "1920x1080",
                    BrowserLanguage = "en-US",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    Country = "US",
                    LanguageCode = "en",
                    SportCode = "FB",
                    RedirectURL = "https://example.com",
                    Status = 1,
                    DeviceType = 1,
                    AffMediaLink = "https://media.example.com",
                    ChannelCode = "CH001"
                }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
            Assert.NotNull(JsonConvert.SerializeObject(result));
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        [Fact]
        public async Task GetClickStatisticAsync_WhenRepositoryReturnsEmpty_ReturnEmptyList()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria 
            { 
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };
            var emptyResult = new List<ClickStatistic>();

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(emptyResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        [Fact]
        public async Task GetClickStatisticAsync_WhenRepositoryReturnsNull_ReturnNull()
        {
            // Arrange
            var criteria = new ClickStatisticCriteria 
            { 
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns((IEnumerable<ClickStatistic>)null);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.Null(result);
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(1, 500)]
        [InlineData(10, 1000)]
        [InlineData(100, 50)]
        public async Task GetClickStatisticAsync_WithDifferentPagination_CallRepositoryCorrectly(int pageIndex, int pageSize)
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

            var expectedResult = new List<ClickStatistic>
            {
                new ClickStatistic { ClickId = 1001, AffId = 123, AffCode = "AFF001" }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        [Fact]
        public async Task GetClickStatisticAsync_WithDateRange_CallRepositoryCorrectly()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var criteria = new ClickStatisticCriteria 
            { 
                LicenseeCode = "12B",
                FromDate = fromDate,
                ToDate = toDate
            };

            var expectedResult = new List<ClickStatistic>
            {
                new ClickStatistic { ClickId = 1001, Date = fromDate.AddDays(5) },
                new ClickStatistic { ClickId = 1002, Date = fromDate.AddDays(10) }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        [Theory]
        [InlineData("12B")]
        [InlineData("N8B")]
        [InlineData("N88")]
        public async Task GetClickStatisticAsync_WithDifferentLicenseeCodes_CallRepositoryCorrectly(string licenseeCode)
        {
            // Arrange
            var criteria = new ClickStatisticCriteria 
            { 
                LicenseeCode = licenseeCode,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            var expectedResult = new List<ClickStatistic>
            {
                new ClickStatistic { ClickId = 1001, AffId = 123, AffCode = "AFF001" }
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(expectedResult);

            var service = new StatisticsService(repository);

            // Action
            var result = await service.GetClickStatisticAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            await repository.Received(1).FetchAsync<ClickStatistic>(criteria);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task StatisticsService_WhenBothMethodsCalled_BothRepositoryMethodsCalled()
        {
            // Arrange
            var mediaChannelCriteria = new AffMediaChannelCriteria { LicenseeCode = "12B" };
            var clickStatisticCriteria = new ClickStatisticCriteria 
            { 
                LicenseeCode = "12B",
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<AffMediaChannel>(Arg.Any<AffMediaChannelCriteria>()).Returns(new List<AffMediaChannel>());
            repository.FetchAsync<ClickStatistic>(Arg.Any<ClickStatisticCriteria>()).Returns(new List<ClickStatistic>());

            var service = new StatisticsService(repository);

            // Action
            var mediaChannelResult = await service.GetAffMediaChannelAsync(mediaChannelCriteria);
            var clickStatisticResult = await service.GetClickStatisticAsync(clickStatisticCriteria);

            // Assert
            Assert.NotNull(mediaChannelResult);
            Assert.NotNull(clickStatisticResult);
            await repository.Received(1).FetchAsync<AffMediaChannel>(mediaChannelCriteria);
            await repository.Received(1).FetchAsync<ClickStatistic>(clickStatisticCriteria);
        }

        #endregion
    }
} 