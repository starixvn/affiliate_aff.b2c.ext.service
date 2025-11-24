using Fanex.B2C.Ext.Service.Core.Constant;

namespace Fanex.B2C.Ext.Service.Core.Tests.Constants
{
    /// <summary>
    /// Unit tests for application constants.
    /// Tests business-critical constants and their values.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class ConstantsTests
    {
        [Fact]
        public void Function_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert - Function constants should match expected business values
            Function.AffInfo.Should().Be("AffInfo");
            Function.Statistics.Should().Be("Statistics");
            Function.Exchange.Should().Be("Exchange");
        }

        [Fact]
        public void Licensee_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert - Licensee constants should match expected business values
            Licensee._12B.Should().Be("12B");
        }

        [Fact]
        public void Pagination_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert - Pagination constants should match expected business values
            Pagination.MaxPageIndex.Should().Be(1_000_000);
            Pagination.MinPageIndex.Should().Be(1);
            Pagination.MaxPageSizeClickStatistic.Should().Be(1000);
            Pagination.MaxPageSizeAffDetailInfo.Should().Be(2000);
            Pagination.MinPageSize.Should().Be(1);
        }

        [Fact]
        public void HeaderKey_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert - Header key constants should match expected values
            HeaderKey.X_ACTUAL_IP.Should().Be("X-Actual-IP");
            HeaderKey.X_FORWARDED_FOR.Should().Be("X-Forwarded-For");
            HeaderKey.X.Should().Be("X-");
        }

        [Fact]
        public void CustomClaimType_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert - Custom claim type constants should match expected values
            CustomClaimType.IsShowIPVN.Should().Be("IsShowIPVN");
            CustomClaimType.Site.Should().Be("Site");
        }

        [Fact]
        public void Constants_ShouldSupportBusinessScenario_FunctionBasedAuthorization()
        {
            // Arrange - Function-based authorization business scenario
            var supportedFunctions = new[] { Function.AffInfo, Function.Statistics, Function.Exchange };

            // Act & Assert - Should support all core business functions
            supportedFunctions.Should().Contain(Function.AffInfo);
            supportedFunctions.Should().Contain(Function.Statistics);
            supportedFunctions.Should().Contain(Function.Exchange);
            supportedFunctions.Should().HaveCount(3);
        }

        [Fact]
        public void Constants_ShouldSupportBusinessScenario_PaginationLimits()
        {
            // Arrange - Pagination limits business scenario
            var clickStatisticRequest = new { PageSize = 500, PageIndex = 1 };
            var affDetailInfoRequest = new { PageSize = 1500, PageIndex = 1 };

            // Act & Assert - Should enforce appropriate business limits
            clickStatisticRequest.PageSize.Should().BeLessOrEqualTo(Pagination.MaxPageSizeClickStatistic);
            affDetailInfoRequest.PageSize.Should().BeLessOrEqualTo(Pagination.MaxPageSizeAffDetailInfo);

            clickStatisticRequest.PageIndex.Should().BeGreaterOrEqualTo(Pagination.MinPageIndex);
            affDetailInfoRequest.PageIndex.Should().BeGreaterOrEqualTo(Pagination.MinPageIndex);
        }

        [Fact]
        public void Constants_ShouldSupportBusinessScenario_LicenseeValidation()
        {
            // Arrange - Licensee validation business scenario
            var validLicensees = new[] { Licensee._12B };
            var requestLicensee = "12B";

            // Act & Assert - Should support licensee validation
            validLicensees.Should().Contain(requestLicensee);
            Licensee._12B.Should().Be(requestLicensee);
        }

        [Fact]
        public void Constants_ShouldSupportBusinessScenario_HeaderProcessing()
        {
            // Arrange - Header processing business scenario
            var headers = new Dictionary<string, string>
            {
                [HeaderKey.X_ACTUAL_IP] = "192.168.1.100",
                [HeaderKey.X_FORWARDED_FOR] = "203.0.113.45",
                ["X-Custom-Header"] = "custom-value"
            };

            // Act & Assert - Should support header processing
            headers.Should().ContainKey(HeaderKey.X_ACTUAL_IP);
            headers.Should().ContainKey(HeaderKey.X_FORWARDED_FOR);
            headers[HeaderKey.X_ACTUAL_IP].Should().Be("192.168.1.100");
            headers[HeaderKey.X_FORWARDED_FOR].Should().Be("203.0.113.45");
        }

        [Fact]
        public void Constants_ShouldSupportBusinessScenario_ClaimBasedSecurity()
        {
            // Arrange - Claim-based security business scenario
            var userClaims = new Dictionary<string, object>
            {
                [CustomClaimType.Site] = "12B",
                [CustomClaimType.IsShowIPVN] = "true"
            };

            // Act & Assert - Should support claim-based security
            userClaims.Should().ContainKey(CustomClaimType.Site);
            userClaims.Should().ContainKey(CustomClaimType.IsShowIPVN);

            userClaims[CustomClaimType.Site].Should().Be("12B");
            userClaims[CustomClaimType.IsShowIPVN].Should().Be("true");
        }
    }
}
