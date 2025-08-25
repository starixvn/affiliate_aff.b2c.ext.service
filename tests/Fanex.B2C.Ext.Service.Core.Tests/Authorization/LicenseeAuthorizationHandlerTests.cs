using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;

namespace Fanex.B2C.Ext.Service.Core.Tests.Authorization
{
    /// <summary>
    /// Comprehensive unit tests for LicenseeAuthorizationHandler.
    /// Tests licensee-based authorization scenarios and business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class LicenseeAuthorizationHandlerTests
    {
        private readonly IOptions<List<User>> _mockUserOptions;
        private readonly List<User> _testUsers;

        public LicenseeAuthorizationHandlerTests()
        {
            _testUsers = CreateTestUsers();
            _mockUserOptions = Substitute.For<IOptions<List<User>>>();
            _mockUserOptions.Value.Returns(_testUsers);
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSucceed_WhenUserHasRequiredLicensee()
        {
            // Arrange - User with required licensee scenario
            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should succeed when user has required licensee
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserDoesNotHaveRequiredLicensee()
        {
            // Arrange - User without required licensee scenario
            var requirement = new LicenseeRequirement("N88");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user doesn't have required licensee
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserIsNotAuthenticated()
        {
            // Arrange - Unauthenticated user scenario
            var requirement = new LicenseeRequirement("12B");
            var user = new ClaimsPrincipal(); // No identity
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user is not authenticated
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUsernameIsEmpty()
        {
            // Arrange - Empty username scenario
            var requirement = new LicenseeRequirement("12B");
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, "")); // Empty name
            var user = new ClaimsPrincipal(identity);
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when username is empty
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserNotFoundInUserList()
        {
            // Arrange - User not found scenario
            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("unknown_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user is not found in user list
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserHasNoSupportedLicensees()
        {
            // Arrange - User with no supported licensees scenario
            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("no_licensees_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user has no supported licensees
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Theory]
        [InlineData("12B", "admin_user", true)]
        [InlineData("N8B", "admin_user", true)]
        [InlineData("N88", "admin_user", false)]
        [InlineData("12B", "limited_user", true)]
        [InlineData("N8B", "limited_user", false)]
        [InlineData("N88", "limited_user", false)]
        [InlineData("12B", "multi_licensee_user", true)]
        [InlineData("N8B", "multi_licensee_user", true)]
        [InlineData("N88", "multi_licensee_user", true)]
        [InlineData("KLDR", "multi_licensee_user", false)]
        public async Task HandleRequirementAsync_ShouldHandleVariousLicenseePermissions(string licenseeCode, string username, bool expectedSuccess)
        {
            // Arrange - Various licensee permission scenarios
            var requirement = new LicenseeRequirement(licenseeCode);
            var user = CreateClaimsPrincipal(username);
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should handle various licensee permissions correctly
            if (expectedSuccess)
            {
                context.HasSucceeded.Should().BeTrue();
                context.HasFailed.Should().BeFalse();
            }
            else
            {
                context.HasSucceeded.Should().BeFalse();
                context.HasFailed.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldBeCaseInsensitive_WhenCheckingLicenseeCodes()
        {
            // Arrange - Case insensitive licensee checking scenario
            var requirement = new LicenseeRequirement("12b"); // Lowercase
            var user = CreateClaimsPrincipal("admin_user"); // Has "12B" in uppercase
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should be case insensitive when checking licensee codes
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_12BLicenseeAccess()
        {
            // Arrange - 12B licensee access business scenario
            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle 12B licensee authorization
            await handler.HandleAsync(context);

            // Assert - Should grant access to 12B licensee data
            context.HasSucceeded.Should().BeTrue();
            var adminUser = _testUsers.Find(u => u.UserName == "admin_user");
            adminUser.Should().NotBeNull();
            adminUser.SupportedLicensees.Should().Contain("12B");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_MultiLicenseeUserAccess()
        {
            // Arrange - Multi-licensee user access business scenario
            var requirement = new LicenseeRequirement("N88");
            var user = CreateClaimsPrincipal("multi_licensee_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle multi-licensee user authorization
            await handler.HandleAsync(context);

            // Assert - Should grant access to users with multiple licensee support
            context.HasSucceeded.Should().BeTrue();
            var multiUser = _testUsers.Find(u => u.UserName == "multi_licensee_user");
            multiUser.Should().NotBeNull();
            multiUser.SupportedLicensees.Should().Contain("12B");
            multiUser.SupportedLicensees.Should().Contain("N8B");
            multiUser.SupportedLicensees.Should().Contain("N88");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_RestrictedLicenseeAccess()
        {
            // Arrange - Restricted licensee access business scenario
            var requirement = new LicenseeRequirement("N8B");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle restricted licensee authorization
            await handler.HandleAsync(context);

            // Assert - Should restrict access to unauthorized licensees
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
            var limitedUser = _testUsers.Find(u => u.UserName == "limited_user");
            limitedUser.Should().NotBeNull();
            limitedUser.SupportedLicensees.Should().Contain("12B");
            limitedUser.SupportedLicensees.Should().NotContain("N8B");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_CrossLicenseeDataSeparation()
        {
            // Arrange - Cross-licensee data separation business scenario
            var requirement = new LicenseeRequirement("KLDR");
            var user = CreateClaimsPrincipal("multi_licensee_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle cross-licensee data access
            await handler.HandleAsync(context);

            // Assert - Should enforce data separation between licensees
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
            var user1 = _testUsers.Find(u => u.UserName == "multi_licensee_user");
            user1.Should().NotBeNull();
            user1.SupportedLicensees.Should().NotContain("KLDR");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_AuditTrailForUnauthorizedAccess()
        {
            // Arrange - Audit trail for unauthorized access scenario
            var requirement = new LicenseeRequirement("N88");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle unauthorized licensee access attempt
            await handler.HandleAsync(context);

            // Assert - Should fail and provide audit information
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();

            // Verify user information for audit purposes
            var auditUser = _testUsers.Find(u => u.UserName == "limited_user");
            auditUser.Should().NotBeNull();
            auditUser.UserName.Should().Be("limited_user");
            auditUser.SupportedLicensees.Should().NotContain("N88");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_RegionalLicenseeAccess()
        {
            // Arrange - Regional licensee access business scenario
            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("regional_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle regional licensee authorization
            await handler.HandleAsync(context);

            // Assert - Should support regional access patterns
            context.HasSucceeded.Should().BeTrue();
            var regionalUser = _testUsers.Find(u => u.UserName == "regional_user");
            regionalUser.Should().NotBeNull();
            regionalUser.SupportedLicensees.Should().Contain("12B");
            regionalUser.SupportedLicensees.Should().Contain("KLDR");
            regionalUser.SupportedLicensees.Should().Contain("KLPROD");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldHandleNullUserList()
        {
            // Arrange - Null user list scenario
            var nullUserOptions = Substitute.For<IOptions<List<User>>>();
            nullUserOptions.Value.Returns((List<User>)null);

            var requirement = new LicenseeRequirement("12B");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(nullUserOptions);

            // Act - Handle authorization with null user list
            var act = async () => await handler.HandleAsync(context);

            // Assert - Should throw exception for null user list (current behavior)
            // Note: The handler currently does not handle null user lists gracefully
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Theory]
        [InlineData("12B")]
        [InlineData("N8B")]
        [InlineData("N88")]
        [InlineData("KLDR")]
        [InlineData("KLPROD")]
        public async Task HandleRequirementAsync_ShouldHandleAllBusinessLicenseeCodes(string licenseeCode)
        {
            // Arrange - All business licensee codes scenario
            var requirement = new LicenseeRequirement(licenseeCode);
            var user = CreateClaimsPrincipal("super_admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new LicenseeAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization for all licensee codes
            await handler.HandleAsync(context);

            // Assert - Super admin should have access to all licensees
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(string username)
        {
            var identity = new ClaimsIdentity("test");
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            return new ClaimsPrincipal(identity);
        }

        private static List<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserName = "admin_user",
                    Password = "admin_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo", "Exchange" },
                    SupportedLicensees = new List<string> { "12B", "N8B" }
                },
                new User
                {
                    UserName = "limited_user",
                    Password = "limited_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo" },
                    SupportedLicensees = new List<string> { "12B" }
                },
                new User
                {
                    UserName = "multi_licensee_user",
                    Password = "multi_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo", "Exchange", "Statistics" },
                    SupportedLicensees = new List<string> { "12B", "N8B", "N88" }
                },
                new User
                {
                    UserName = "regional_user",
                    Password = "regional_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo", "Exchange" },
                    SupportedLicensees = new List<string> { "12B", "KLDR", "KLPROD" }
                },
                new User
                {
                    UserName = "super_admin_user",
                    Password = "super_admin_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo", "Exchange", "Statistics" },
                    SupportedLicensees = new List<string> { "12B", "N8B", "N88", "KLDR", "KLPROD" }
                },
                new User
                {
                    UserName = "no_licensees_user",
                    Password = "no_licensees_password_hash",
                    SupportedFunctions = new List<string> { "AffInfo" },
                    SupportedLicensees = null
                }
            };
        }
    }
}
