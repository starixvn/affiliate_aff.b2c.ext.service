using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.B2C.Ext.Service.Core.Tests.Extensions
{
    /// <summary>
    /// Comprehensive unit tests for AuthorizationExtensions.
    /// Tests service registration and policy configuration for authorization.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class AuthorizationExtensionsTests
    {
        [Fact]
        public void AddCustomAuthorization_ShouldRegisterAuthorizationHandlers()
        {
            // Arrange - Service collection setup scenario
            var services = new ServiceCollection();

            // Act - Add custom authorization services
            var result = services.AddCustomAuthorization();

            // Assert - Should register authorization handlers
            result.Should().NotBeNull();
            result.Should().BeSameAs(services);

            // Verify that authorization handlers are registered in service collection
            var handlerDescriptors = services.Where(s => s.ServiceType == typeof(IAuthorizationHandler)).ToList();
            handlerDescriptors.Should().HaveCount(2, "Should register both FunctionAuthorizationHandler and LicenseeAuthorizationHandler");

            var implementationTypes = handlerDescriptors.Select(h => h.ImplementationType).ToList();
            implementationTypes.Should().Contain(typeof(FunctionAuthorizationHandler));
            implementationTypes.Should().Contain(typeof(LicenseeAuthorizationHandler));
        }

        [Fact]
        public void AddCustomAuthorization_ShouldSupportBusinessScenario_B2CApiAuthorization()
        {
            // Arrange - B2C API authorization business scenario
            var services = new ServiceCollection();

            // Act - Configure authorization for B2C API
            services.AddCustomAuthorization();

            // Assert - Should have both function and licensee authorization handlers registered
            var handlerDescriptors = services.Where(s => s.ServiceType == typeof(IAuthorizationHandler)).ToList();
            handlerDescriptors.Should().HaveCount(2, "Should register authorization handlers for B2C API");

            var implementationTypes = handlerDescriptors.Select(h => h.ImplementationType).ToList();
            implementationTypes.Should().Contain(typeof(FunctionAuthorizationHandler));
            implementationTypes.Should().Contain(typeof(LicenseeAuthorizationHandler));
        }

        [Fact]
        public void AddCustomAuthorization_ShouldSupportBusinessScenario_MultiTenantAuthorization()
        {
            // Arrange - Multi-tenant authorization business scenario
            var services = new ServiceCollection();

            // Act - Configure multi-tenant authorization
            services.AddCustomAuthorization();

            // Assert - Should support multiple authorization handlers for different tenants
            var handlerDescriptors = services.Where(s => s.ServiceType == typeof(IAuthorizationHandler)).ToList();

            // Should have at least function and licensee handlers for multi-tenant support
            handlerDescriptors.Should().HaveCount(2, "Should register handlers for multi-tenant support");

            // Verify specific handler types are registered
            var implementationTypes = handlerDescriptors.Select(h => h.ImplementationType).ToList();
            implementationTypes.Should().Contain(typeof(FunctionAuthorizationHandler));
            implementationTypes.Should().Contain(typeof(LicenseeAuthorizationHandler));
        }

        [Fact]
        public void AddCustomPolicies_ShouldAddAffInfoPolicy()
        {
            // Arrange - AffInfo policy configuration scenario
            var options = new AuthorizationOptions();

            // Act - Add custom policies
            var result = options.AddCustomPolicies();

            // Assert - Should add AffInfo policy
            result.Should().NotBeNull();
            result.Should().BeSameAs(options);

            var affInfoPolicy = options.GetPolicy("AffInfo");
            affInfoPolicy.Should().NotBeNull();
            affInfoPolicy.Requirements.Should().HaveCount(1);
            affInfoPolicy.Requirements.First().Should().BeOfType<FunctionRequirement>();
        }

        [Fact]
        public void AddCustomPolicies_ShouldAddStatisticsPolicy()
        {
            // Arrange - Statistics policy configuration scenario
            var options = new AuthorizationOptions();

            // Act - Add custom policies
            options.AddCustomPolicies();

            // Assert - Should add Statistics policy
            var statisticsPolicy = options.GetPolicy("Statistics");
            statisticsPolicy.Should().NotBeNull();
            statisticsPolicy.Requirements.Should().HaveCount(1);
            statisticsPolicy.Requirements.First().Should().BeOfType<FunctionRequirement>();
        }

        [Fact]
        public void AddCustomPolicies_ShouldAddExchangePolicy()
        {
            // Arrange - Exchange policy configuration scenario
            var options = new AuthorizationOptions();

            // Act - Add custom policies
            options.AddCustomPolicies();

            // Assert - Should add Exchange policy
            var exchangePolicy = options.GetPolicy("Exchange");
            exchangePolicy.Should().NotBeNull();
            exchangePolicy.Requirements.Should().HaveCount(1);
            exchangePolicy.Requirements.First().Should().BeOfType<FunctionRequirement>();
        }

        [Fact]
        public void AddCustomPolicies_ShouldSupportBusinessScenario_B2CApiPolicyConfiguration()
        {
            // Arrange - B2C API policy configuration business scenario
            var options = new AuthorizationOptions();

            // Act - Configure B2C API policies
            options.AddCustomPolicies();

            // Assert - Should have all required B2C API policies
            var affInfoPolicy = options.GetPolicy("AffInfo");
            var statisticsPolicy = options.GetPolicy("Statistics");
            var exchangePolicy = options.GetPolicy("Exchange");

            affInfoPolicy.Should().NotBeNull("AffInfo policy is required for affiliate information endpoints");
            statisticsPolicy.Should().NotBeNull("Statistics policy is required for statistics endpoints");
            exchangePolicy.Should().NotBeNull("Exchange policy is required for exchange rate endpoints");

            // All policies should have function requirements
            affInfoPolicy.Requirements.Should().AllBeOfType<FunctionRequirement>();
            statisticsPolicy.Requirements.Should().AllBeOfType<FunctionRequirement>();
            exchangePolicy.Requirements.Should().AllBeOfType<FunctionRequirement>();
        }

        [Fact]
        public void AddCustomPolicies_ShouldSupportBusinessScenario_FunctionBasedAuthorization()
        {
            // Arrange - Function-based authorization business scenario
            var options = new AuthorizationOptions();

            // Act - Configure function-based authorization
            options.AddCustomPolicies();

            // Assert - Should create policies with function requirements
            var policies = new[] { "AffInfo", "Statistics", "Exchange" };

            foreach (var policyName in policies)
            {
                var policy = options.GetPolicy(policyName);
                policy.Should().NotBeNull($"{policyName} policy should exist");
                policy.Requirements.Should().HaveCount(1, $"{policyName} policy should have exactly one requirement");
                policy.Requirements.First().Should().BeOfType<FunctionRequirement>($"{policyName} policy should use FunctionRequirement");
            }
        }

        [Fact]
        public void AddCustomPolicies_ShouldSupportBusinessScenario_LicenseeSpecificAccess()
        {
            // Arrange - Licensee-specific access business scenario
            var options = new AuthorizationOptions();

            // Act - Configure policies for licensee-specific access
            options.AddCustomPolicies();

            // Assert - Should support policies that can be combined with licensee requirements
            var affInfoPolicy = options.GetPolicy("AffInfo");
            var statisticsPolicy = options.GetPolicy("Statistics");
            var exchangePolicy = options.GetPolicy("Exchange");

            var policies = new[] { affInfoPolicy, statisticsPolicy, exchangePolicy };
            policies.Should().AllSatisfy(p => p.Should().NotBeNull());

            // Each policy should support function-based authorization that can work with licensee authorization
            foreach (var policy in policies)
            {
                policy.Requirements.Should().NotBeEmpty($"Policy should have requirements");
                policy.Requirements.Should().AllBeOfType<FunctionRequirement>($"Policy should use FunctionRequirement");
            }
        }

        [Theory]
        [InlineData("AffInfo")]
        [InlineData("Statistics")]
        [InlineData("Exchange")]
        public void AddCustomPolicies_ShouldCreateValidPolicy_ForEachFunction(string policyName)
        {
            // Arrange - Policy validation scenario
            var options = new AuthorizationOptions();

            // Act - Add custom policies
            options.AddCustomPolicies();

            // Assert - Should create valid policy for each function
            var policy = options.GetPolicy(policyName);
            policy.Should().NotBeNull($"{policyName} policy should be created");
            policy.Requirements.Should().HaveCount(1, $"{policyName} policy should have exactly one requirement");
            policy.Requirements.First().Should().BeOfType<FunctionRequirement>($"{policyName} policy should use FunctionRequirement");
        }

        [Fact]
        public void AddCustomPolicies_ShouldSupportBusinessScenario_ApiEndpointSecurity()
        {
            // Arrange - API endpoint security business scenario
            var options = new AuthorizationOptions();

            // Act - Configure API endpoint security
            options.AddCustomPolicies();

            // Assert - Should provide comprehensive security policies for all API endpoints
            var requiredPolicies = new[] { "AffInfo", "Statistics", "Exchange" };

            foreach (var requiredPolicy in requiredPolicies)
            {
                var policy = options.GetPolicy(requiredPolicy);
                policy.Should().NotBeNull($"{requiredPolicy} policy is required for API endpoint security");

                // Each policy should have appropriate requirements for security
                policy.Requirements.Should().NotBeEmpty($"{requiredPolicy} policy should have security requirements");
                policy.Requirements.Should().AllBeOfType<FunctionRequirement>($"{requiredPolicy} should use function-based security");
            }
        }

        [Fact]
        public void AddCustomPolicies_ShouldSupportBusinessScenario_RoleBasedAccessControl()
        {
            // Arrange - Role-based access control business scenario
            var options = new AuthorizationOptions();

            // Act - Configure role-based access control
            options.AddCustomPolicies();

            // Assert - Should support different access levels for different functions
            var affInfoPolicy = options.GetPolicy("AffInfo");
            var statisticsPolicy = options.GetPolicy("Statistics");
            var exchangePolicy = options.GetPolicy("Exchange");

            // Should have distinct policies for different functional areas
            affInfoPolicy.Should().NotBeNull("Should have affiliate information access policy");
            statisticsPolicy.Should().NotBeNull("Should have statistics access policy");
            exchangePolicy.Should().NotBeNull("Should have exchange rate access policy");

            // Each policy represents a different access level/role
            var policies = new[] { affInfoPolicy, statisticsPolicy, exchangePolicy };
            policies.Should().HaveCount(3, "Should have at least 3 distinct access policies");
        }
    }
}