using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;

namespace Fanex.B2C.Ext.Service.Core.Tests.Authorization
{
    /// <summary>
    /// Comprehensive unit tests for FunctionAuthorizationHandler.
    /// Tests function-based authorization scenarios and business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class FunctionAuthorizationHandlerTests
    {
        private readonly IOptions<List<User>> _mockUserOptions;
        private readonly List<User> _testUsers;

        public FunctionAuthorizationHandlerTests()
        {
            _testUsers = CreateTestUsers();
            _mockUserOptions = Substitute.For<IOptions<List<User>>>();
            _mockUserOptions.Value.Returns(_testUsers);
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSucceed_WhenUserHasRequiredFunction()
        {
            // Arrange - User with required function scenario
            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should succeed when user has required function
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserDoesNotHaveRequiredFunction()
        {
            // Arrange - User without required function scenario
            var requirement = new FunctionRequirement("Statistics");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user doesn't have required function
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserIsNotAuthenticated()
        {
            // Arrange - Unauthenticated user scenario
            var requirement = new FunctionRequirement("AffInfo");
            var user = new ClaimsPrincipal(); // No identity
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

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
            var requirement = new FunctionRequirement("AffInfo");
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, "")); // Empty name
            var user = new ClaimsPrincipal(identity);
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

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
            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("unknown_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user is not found in user list
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldFail_WhenUserHasNoSupportedFunctions()
        {
            // Arrange - User with no supported functions scenario
            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("no_functions_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should fail when user has no supported functions
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();
        }

        [Theory]
        [InlineData("AffInfo", "admin_user", true)]
        [InlineData("Exchange", "admin_user", true)]
        [InlineData("Statistics", "admin_user", false)]
        [InlineData("AffInfo", "limited_user", true)]
        [InlineData("Exchange", "limited_user", false)]
        [InlineData("Statistics", "limited_user", false)]
        [InlineData("AffInfo", "exchange_user", false)]
        [InlineData("Exchange", "exchange_user", true)]
        [InlineData("Statistics", "exchange_user", false)]
        public async Task HandleRequirementAsync_ShouldHandleVariousFunctionPermissions(string functionName, string username, bool expectedSuccess)
        {
            // Arrange - Various function permission scenarios
            var requirement = new FunctionRequirement(functionName);
            var user = CreateClaimsPrincipal(username);
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should handle various function permissions correctly
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
        public async Task HandleRequirementAsync_ShouldBeCaseInsensitive_WhenCheckingFunctionNames()
        {
            // Arrange - Case insensitive function checking scenario
            var requirement = new FunctionRequirement("AFFINFO"); // Uppercase
            var user = CreateClaimsPrincipal("admin_user"); // Has "AffInfo" in mixed case
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle authorization requirement
            await handler.HandleAsync(context);

            // Assert - Should be case insensitive when checking function names
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_B2CAdminAccess()
        {
            // Arrange - B2C admin access business scenario
            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle B2C admin authorization
            await handler.HandleAsync(context);

            // Assert - Should grant access to admin users for affiliate information
            context.HasSucceeded.Should().BeTrue();
            var adminUser = _testUsers.Find(u => u.UserName == "admin_user");
            adminUser.Should().NotBeNull();
            adminUser.SupportedFunctions.Should().Contain("AffInfo");
            adminUser.SupportedFunctions.Should().Contain("Exchange");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_LimitedUserAccess()
        {
            // Arrange - Limited user access business scenario
            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle limited user authorization
            await handler.HandleAsync(context);

            // Assert - Should grant limited access to specific functions only
            context.HasSucceeded.Should().BeTrue();
            var limitedUser = _testUsers.Find(u => u.UserName == "limited_user");
            limitedUser.Should().NotBeNull();
            limitedUser.SupportedFunctions.Should().Contain("AffInfo");
            limitedUser.SupportedFunctions.Should().NotContain("Exchange");
            limitedUser.SupportedFunctions.Should().NotContain("Statistics");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_ExchangeOnlyAccess()
        {
            // Arrange - Exchange-only access business scenario
            var requirement = new FunctionRequirement("Exchange");
            var user = CreateClaimsPrincipal("exchange_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle exchange user authorization
            await handler.HandleAsync(context);

            // Assert - Should grant access to exchange functions only
            context.HasSucceeded.Should().BeTrue();
            var exchangeUser = _testUsers.Find(u => u.UserName == "exchange_user");
            exchangeUser.Should().NotBeNull();
            exchangeUser.SupportedFunctions.Should().Contain("Exchange");
            exchangeUser.SupportedFunctions.Should().NotContain("AffInfo");
            exchangeUser.SupportedFunctions.Should().NotContain("Statistics");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldSupportBusinessScenario_SecurityAuditTrail()
        {
            // Arrange - Security audit trail scenario
            var requirement = new FunctionRequirement("Statistics");
            var user = CreateClaimsPrincipal("limited_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(_mockUserOptions);

            // Act - Handle unauthorized access attempt
            await handler.HandleAsync(context);

            // Assert - Should fail and provide audit trail information
            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();

            // Verify user information for audit purposes
            var auditUser = _testUsers.Find(u => u.UserName == "limited_user");
            auditUser.Should().NotBeNull();
            auditUser.SupportedFunctions.Should().NotContain("Statistics");
        }

        [Fact]
        public async Task HandleRequirementAsync_ShouldHandleNullUserList()
        {
            // Arrange - Null user list scenario
            var nullUserOptions = Substitute.For<IOptions<List<User>>>();
            nullUserOptions.Value.Returns((List<User>)null);

            var requirement = new FunctionRequirement("AffInfo");
            var user = CreateClaimsPrincipal("admin_user");
            var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);
            var handler = new FunctionAuthorizationHandler(nullUserOptions);

            // Act - Handle authorization with null user list
            var act = async () => await handler.HandleAsync(context);

            // Assert - Should throw exception for null user list (current behavior)
            // Note: The handler currently does not handle null user lists gracefully
            await act.Should().ThrowAsync<NullReferenceException>();
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
                    UserName = "exchange_user",
                    Password = "exchange_password_hash",
                    SupportedFunctions = new List<string> { "Exchange" },
                    SupportedLicensees = new List<string> { "12B", "N8B", "N88" }
                },
                new User
                {
                    UserName = "no_functions_user",
                    Password = "no_functions_password_hash",
                    SupportedFunctions = null,
                    SupportedLicensees = new List<string> { "12B" }
                }
            };
        }
    }
}
