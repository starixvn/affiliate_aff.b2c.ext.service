using Fanex.B2C.Ext.Service.Core.Extensions;

namespace Fanex.B2C.Ext.Service.Core.Tests.Extensions
{
    /// <summary>
    /// Comprehensive unit tests for StringExtensions.
    /// Tests string matching business scenarios and edge cases.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class StringExtensionsTests
    {
        [Fact]
        public void MatchWith_ShouldReturnTrue_WhenStringsAreExactMatch()
        {
            // Arrange - Exact match scenario
            var value = "12B";
            var strToMatch = "12B";

            // Act - Test exact string matching
            var result = value.MatchWith(strToMatch);

            // Assert - Should return true for exact matches
            result.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldReturnTrue_WhenStringsMatchIgnoreCase()
        {
            // Arrange - Case insensitive match scenario
            var value = "12B";
            var strToMatch = "12b";

            // Act - Test case insensitive matching
            var result = value.MatchWith(strToMatch);

            // Assert - Should return true for case insensitive matches
            result.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldReturnFalse_WhenStringsAreDifferent()
        {
            // Arrange - Different strings scenario
            var value = "12B";
            var strToMatch = "N8B";

            // Act - Test different string matching
            var result = value.MatchWith(strToMatch);

            // Assert - Should return false for different strings
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("12B", "12B", true)]
        [InlineData("12B", "12b", true)]
        [InlineData("12B", "12B ", false)] // Whitespace matters
        [InlineData("12B", " 12B", false)] // Leading whitespace matters
        [InlineData("N8B", "N8B", true)]
        [InlineData("N8B", "n8b", true)]
        [InlineData("N8B", "N88", false)]
        [InlineData("KLDR", "kldr", true)]
        [InlineData("KLDR", "KLPROD", false)]
        [InlineData("", "", true)]
        [InlineData("12B", "", false)]
        [InlineData("", "12B", false)]
        public void MatchWith_ShouldHandleVariousStringComparisons(string value, string strToMatch, bool expectedResult)
        {
            // Arrange - Various string comparison scenarios

            // Act - Test various string matching scenarios
            var result = value.MatchWith(strToMatch);

            // Assert - Should handle various string comparisons correctly
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void MatchWith_ShouldReturnFalse_WhenValueIsNull()
        {
            // Arrange - Null value scenario
            string value = null;
            var strToMatch = "12B";

            // Act - Test null value matching
            var act = () => value.MatchWith(strToMatch);

            // Assert - Should return false for null value (string.Equals handles null gracefully)
            var result = act.Invoke();
            result.Should().BeFalse();
        }

        [Fact]
        public void MatchWith_ShouldReturnFalse_WhenStrToMatchIsNull()
        {
            // Arrange - Null strToMatch scenario
            var value = "12B";
            string strToMatch = null;

            // Act - Test null strToMatch matching
            var result = value.MatchWith(strToMatch);

            // Assert - Should return false when strToMatch is null
            result.Should().BeFalse();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_LicenseeCodeMatching()
        {
            // Arrange - Licensee code matching business scenario
            var licenseeCode = "12B";
            var supportedLicensees = new List<string> { "12B", "N8B", "N88" };

            // Act - Test licensee code matching
            var hasAccess = supportedLicensees.Any(supported => licenseeCode.MatchWith(supported));

            // Assert - Should support licensee code matching business logic
            hasAccess.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_FunctionNameMatching()
        {
            // Arrange - Function name matching business scenario
            var requestedFunction = "AffInfo";
            var supportedFunctions = new List<string> { "AFFINFO", "exchange", "Statistics" };

            // Act - Test function name matching
            var hasPermission = supportedFunctions.Any(supported => requestedFunction.MatchWith(supported));

            // Assert - Should support function name matching business logic
            hasPermission.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_EnvironmentConfigurationMatching()
        {
            // Arrange - Environment configuration matching business scenario
            var currentEnvironment = "Production";
            var configuredEnvironments = new List<string> { "development", "PRODUCTION", "Staging" };

            // Act - Test environment configuration matching
            var isValidEnvironment = configuredEnvironments.Any(env => currentEnvironment.MatchWith(env));

            // Assert - Should support environment configuration matching
            isValidEnvironment.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_UserRoleValidation()
        {
            // Arrange - User role validation business scenario
            var userRole = "Admin";
            var allowedRoles = new List<string> { "admin", "Manager", "SuperUser" };

            // Act - Test user role validation
            var isAuthorized = allowedRoles.Any(role => userRole.MatchWith(role));

            // Assert - Should support user role validation logic
            isAuthorized.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_ApiEndpointMatching()
        {
            // Arrange - API endpoint matching business scenario
            var requestedEndpoint = "AffInfo";
            var availableEndpoints = new List<string> { "affinfo", "Exchange", "Statistics" };

            // Act - Test API endpoint matching
            var isValidEndpoint = availableEndpoints.Any(endpoint => requestedEndpoint.MatchWith(endpoint));

            // Assert - Should support API endpoint matching logic
            isValidEndpoint.Should().BeTrue();
        }

        [Theory]
        [InlineData("12B", new[] { "12B", "N8B", "N88" }, true)]
        [InlineData("12b", new[] { "12B", "N8B", "N88" }, true)]
        [InlineData("KLDR", new[] { "12B", "N8B", "N88" }, false)]
        [InlineData("AffInfo", new[] { "AFFINFO", "Exchange", "Statistics" }, true)]
        [InlineData("affinfo", new[] { "AFFINFO", "Exchange", "Statistics" }, true)]
        [InlineData("Reporting", new[] { "AFFINFO", "Exchange", "Statistics" }, false)]
        public void MatchWith_ShouldSupportBusinessScenario_CollectionMatching(string value, string[] collection, bool expectedMatch)
        {
            // Arrange - Collection matching business scenario

            // Act - Test value against collection
            var hasMatch = collection.Any(item => value.MatchWith(item));

            // Assert - Should support collection matching scenarios
            hasMatch.Should().Be(expectedMatch);
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_ConfigurationKeyMatching()
        {
            // Arrange - Configuration key matching business scenario
            var configurationKeys = new Dictionary<string, string>
            {
                ["ConnectionString"] = "server=localhost;",
                ["ApiKey"] = "abc123",
                ["Environment"] = "Production"
            };
            var requestedKey = "environment";

            // Act - Test configuration key matching
            var matchedKey = configurationKeys.Keys.FirstOrDefault(key => requestedKey.MatchWith(key));

            // Assert - Should support configuration key matching
            matchedKey.Should().Be("Environment");
            configurationKeys[matchedKey].Should().Be("Production");
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_MultiLanguageSupport()
        {
            // Arrange - Multi-language support scenario (ASCII only for this extension)
            var supportedLanguages = new List<string> { "EN", "ZH", "JP", "KR" };
            var requestedLanguage = "en";

            // Act - Test multi-language matching
            var isSupported = supportedLanguages.Any(lang => requestedLanguage.MatchWith(lang));

            // Assert - Should support multi-language matching
            isSupported.Should().BeTrue();
        }

        [Fact]
        public void MatchWith_ShouldSupportBusinessScenario_FeatureFlagMatching()
        {
            // Arrange - Feature flag matching business scenario
            var enabledFeatures = new List<string> { "NewUI", "AdvancedReporting", "BetaFeatures" };
            var requestedFeature = "newui";

            // Act - Test feature flag matching
            var isFeatureEnabled = enabledFeatures.Any(feature => requestedFeature.MatchWith(feature));

            // Assert - Should support feature flag matching
            isFeatureEnabled.Should().BeTrue();
        }
    }
}
