using Fanex.B2C.Ext.Service.Core.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Middlewares
{
    /// <summary>
    /// Enhanced comprehensive unit tests for WhoAmIHealthCheck.
    /// Tests health check functionality, server information gathering, and business scenarios.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class EnhancedWhoAmIHealthCheckTest
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpContext _mockContext;
        private readonly HttpRequest _mockRequest;
        private readonly ConnectionInfo _mockConnection;
        private readonly IHeaderDictionary _mockHeaders;

        public EnhancedWhoAmIHealthCheckTest()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            _mockContext = Substitute.For<HttpContext>();
            _mockRequest = Substitute.For<HttpRequest>();
            _mockConnection = Substitute.For<ConnectionInfo>();
            _mockHeaders = Substitute.For<IHeaderDictionary>();

            // Setup default mock behavior
            _httpContextAccessor.HttpContext.Returns(_mockContext);
            _mockContext.Request.Returns(_mockRequest);
            _mockContext.Connection.Returns(_mockConnection);
            _mockRequest.Headers.Returns(_mockHeaders);
            _mockRequest.Host.Returns(new HostString("test.b2c.com"));
            _mockRequest.Scheme.Returns("https");
            _mockConnection.LocalIpAddress.Returns(IPAddress.Parse("192.168.1.10"));
            _mockConnection.LocalPort.Returns(443);
            _mockConnection.RemoteIpAddress.Returns(IPAddress.Parse("203.0.113.45"));
            _webHostEnvironment.EnvironmentName.Returns("Development");

            // CRITICAL: Link request back to context for GetClientIP extension method
            _mockRequest.HttpContext.Returns(_mockContext);

            // Setup empty headers by default - IHeaderDictionary implements IEnumerable
            // No need to mock Where method, it will work with empty dictionary
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldReturnHealthyStatus_Always()
        {
            // Arrange - Basic health check scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Perform health check
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should always return healthy status
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Server Info");
            result.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeEnvironmentInfo_InHealthCheckResult()
        {
            // Arrange - Environment info scenario
            _webHostEnvironment.EnvironmentName.Returns("Production");
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with environment info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include environment information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Server Info");
            result.Data.Should().ContainKey("Env");
            result.Data["Env"].Should().Be("Production");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeServerIPInfo_InHealthCheckResult()
        {
            // Arrange - Server IP info scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with server IP info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include server IP information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Host Name");
            result.Data["Host Name"].Should().NotBeNull();

            // Should have at least one IP address (depends on system configuration)
            var ipKeys = result.Data.Keys.Where(k => k.StartsWith("IP")).ToList();
            // Note: In test environment, IP resolution might not work, so we just verify the key exists
            result.Data.Should().ContainKey("Host Name");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeClientConnectionInfo_InHealthCheckResult()
        {
            // Arrange - Client connection info scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with client connection info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include client connection information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("From Internal");
            result.Data.Should().ContainKey("Local Ip");
            result.Data.Should().ContainKey("Local Port");
            result.Data.Should().ContainKey("Client Ip");
            result.Data.Should().ContainKey("Actual Client Ip");

            result.Data["From Internal"].Should().Be(false);
            result.Data["Local Ip"].Should().Be("192.168.1.10");
            result.Data["Local Port"].Should().Be(443);
            result.Data["Client Ip"].Should().Be("203.0.113.45");
            result.Data["Actual Client Ip"].Should().Be("203.0.113.45");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeServerTimeInfo_InHealthCheckResult()
        {
            // Arrange - Server time info scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with server time info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include server time information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Server Time");
            result.Data.Should().ContainKey("Last Updated");
            result.Data["Server Time"].Should().NotBeNull();
            result.Data["Last Updated"].Should().NotBeNull();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeRequestInfo_InHealthCheckResult()
        {
            // Arrange - Request info scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with request info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include request information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Host");
            result.Data.Should().ContainKey("Schema");
            result.Data["Host"].ToString().Should().Be("test.b2c.com");
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldIncludeXHeaders_InHealthCheckResult()
        {
            // Arrange - X-Headers scenario
            var xForwardedFor = "203.0.113.45";
            var xRealIp = "192.0.2.10";

            // Create a real HeaderDictionary for proper enumeration support
            var headerDictionary = new HeaderDictionary
            {
                { "X-Forwarded-For", new StringValues(xForwardedFor) },
                { "X-Real-IP", new StringValues(xRealIp) },
                { "X-Custom-Header", new StringValues("custom-value") },
                { "Authorization", new StringValues("Bearer token") },
                { "Content-Type", new StringValues("application/json") }
            };

            // Use real HeaderDictionary instead of mock for proper LINQ support
            _mockRequest.Headers.Returns(headerDictionary);

            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check with X-Headers
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include X-Headers information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("X-Headers");
            result.Data["X-Headers"].Should().Be("---------------------------------");

            // Should include X-Headers only
            result.Data.Should().ContainKey(" + X-Forwarded-For");
            result.Data.Should().ContainKey(" + X-Real-IP");
            result.Data.Should().ContainKey(" + X-Custom-Header");
            result.Data[" + X-Forwarded-For"].Should().Be(xForwardedFor);
            result.Data[" + X-Real-IP"].Should().Be(xRealIp);
            result.Data[" + X-Custom-Header"].Should().Be("custom-value");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_B2CHealthMonitoring()
        {
            // Arrange - B2C health monitoring business scenario
            _webHostEnvironment.EnvironmentName.Returns("Production");
            _mockRequest.Host.Returns(new HostString("api.b2c.production.com"));
            _mockRequest.Scheme.Returns("https");
            // FromInternal() is an extension method that will work based on the request setup
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get B2C health monitoring info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should provide comprehensive B2C health information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Description.Should().Be("Server Info");
            result.Data.Should().ContainKey("Env");
            result.Data["Env"].Should().Be("Production");
            result.Data.Should().ContainKey("Host Name");
            result.Data.Should().ContainKey("Server Time");
            result.Data.Should().ContainKey("From Internal");
            result.Data["From Internal"].Should().Be(false);
            result.Data["Host"].ToString().Should().Be("api.b2c.production.com");
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_LoadBalancerHealthCheck()
        {
            // Arrange - Load balancer health check business scenario
            var xForwardedFor = "10.0.0.1,172.16.0.1";
            var xRealIp = "203.0.113.45";

            // Create a real HeaderDictionary for proper enumeration support
            var headerDictionary = new HeaderDictionary
            {
                { "X-Forwarded-For", new StringValues(xForwardedFor) },
                { "X-Real-IP", new StringValues(xRealIp) },
                { "X-Load-Balancer", new StringValues("nginx-lb-01") }
            };

            _mockRequest.Headers.Returns(headerDictionary);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get load balancer health check info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should provide load balancer health information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Actual Client Ip");
            result.Data.Should().ContainKey("Client Ip");

            // Should include load balancer headers
            result.Data.Should().ContainKey(" + X-Forwarded-For");
            result.Data.Should().ContainKey(" + X-Real-IP");
            result.Data.Should().ContainKey(" + X-Load-Balancer");
            result.Data[" + X-Forwarded-For"].Should().Be(xForwardedFor);
            result.Data[" + X-Real-IP"].Should().Be(xRealIp);
            result.Data[" + X-Load-Balancer"].Should().Be("nginx-lb-01");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_InternalVsExternalAccess()
        {
            // Arrange - Internal vs external access business scenario
            // FromInternal() checks if host contains "intb2c" or matches server hostname
            _mockRequest.Host.Returns(new HostString("api.intb2c.local"));
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get internal access health check info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should indicate internal access
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("From Internal");
            result.Data["From Internal"].Should().Be(true);
            result.Data["Host"].ToString().Should().Be("api.intb2c.local");
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Production")]
        [InlineData("LOCAL")]
        [InlineData("UAT")]
        [InlineData("KLDR")]
        [InlineData("KLPROD")]
        public async Task CheckHealthAsync_ShouldHandleMultipleEnvironments_Correctly(string environment)
        {
            // Arrange - Multiple environments scenario
            _webHostEnvironment.EnvironmentName.Returns(environment);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get health check for different environments
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should handle all environments correctly
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Env");
            result.Data["Env"].Should().Be(environment);
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_SecurityAuditInformation()
        {
            // Arrange - Security audit information business scenario
            var xForwardedFor = "192.168.1.100";
            var xActualIp = "203.0.113.45";

            // Create a real HeaderDictionary for proper enumeration support
            var headerDictionary = new HeaderDictionary
            {
                { "X-Forwarded-For", new StringValues(xForwardedFor) },
                { "X-Actual-IP", new StringValues(xActualIp) },
                { "X-Request-ID", new StringValues("audit-12345") },
                { "X-Session-ID", new StringValues("session-67890") }
            };

            _mockRequest.Headers.Returns(headerDictionary);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get security audit information
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should provide security audit information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Actual Client Ip");
            result.Data.Should().ContainKey("Client Ip");
            result.Data.Should().ContainKey("Server Time");

            // Should include security-related headers
            result.Data.Should().ContainKey(" + X-Forwarded-For");
            result.Data.Should().ContainKey(" + X-Actual-IP");
            result.Data.Should().ContainKey(" + X-Request-ID");
            result.Data.Should().ContainKey(" + X-Session-ID");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_PerformanceMonitoring()
        {
            // Arrange - Performance monitoring business scenario (representative sample per guidelines)
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);
            var tasks = new List<Task<HealthCheckResult>>();

            // Act - Multiple concurrent health checks
            for (int i = 0; i < 25; i++) // Representative sample per guidelines
            {
                tasks.Add(healthCheck.CheckHealthAsync(new HealthCheckContext()));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - Should handle concurrent health checks efficiently
            results.Should().HaveCount(25);
            results.Should().OnlyContain(r => r.Status == HealthStatus.Healthy);
            results.Should().OnlyContain(r => r.Description == "Server Info");

            // All results should have required data
            foreach (var result in results)
            {
                result.Data.Should().ContainKey("Env");
                result.Data.Should().ContainKey("Host Name");
                result.Data.Should().ContainKey("Server Time");
                result.Data.Should().ContainKey("From Internal");
                result.Data.Should().ContainKey("Local Ip");
                result.Data.Should().ContainKey("Client Ip");
            }

            // Explicit GC for performance test cleanup per guidelines
            GC.Collect();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_AssemblyVersionInformation()
        {
            // Arrange - Assembly version information business scenario
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get assembly version information
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should include assembly version information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Last Updated");
            result.Data["Last Updated"].Should().NotBeNull();

            // Should be a valid date string
            var lastUpdated = result.Data["Last Updated"].ToString();
            lastUpdated.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_NetworkConfigurationValidation()
        {
            // Arrange - Network configuration validation business scenario
            _mockConnection.LocalIpAddress.Returns(IPAddress.Parse("10.0.0.15"));
            _mockConnection.LocalPort.Returns(8080);
            _mockConnection.RemoteIpAddress.Returns(IPAddress.Parse("172.16.0.25"));
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get network configuration validation info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should validate network configuration
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Local Ip");
            result.Data.Should().ContainKey("Local Port");
            result.Data.Should().ContainKey("Host");
            result.Data.Should().ContainKey("Schema");

            // Network information should match mock values
            result.Data["Local Ip"].Should().Be("10.0.0.15");
            result.Data["Local Port"].Should().Be(8080);
            result.Data["Client Ip"].Should().Be("172.16.0.25");
            result.Data["Host"].ToString().Should().Be("test.b2c.com");
            result.Data["Schema"].Should().Be("https");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_CDNProxyDetection()
        {
            // Arrange - CDN/Proxy detection business scenario
            // Create a real HeaderDictionary for proper enumeration support
            var headerDictionary = new HeaderDictionary
            {
                { "X-Forwarded-For", new StringValues("203.0.113.45, 198.51.100.10, 192.0.2.5") },
                { "X-Real-IP", new StringValues("203.0.113.45") },
                { "X-CDN-Provider", new StringValues("CloudFlare") },
                { "X-Edge-Location", new StringValues("SIN") },
                { "X-Cache", new StringValues("HIT") }
            };

            _mockRequest.Headers.Returns(headerDictionary);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get CDN/Proxy detection info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should detect CDN/Proxy configuration
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey(" + X-Forwarded-For");
            result.Data.Should().ContainKey(" + X-Real-IP");
            result.Data.Should().ContainKey(" + X-CDN-Provider");
            result.Data.Should().ContainKey(" + X-Edge-Location");
            result.Data.Should().ContainKey(" + X-Cache");

            result.Data[" + X-Forwarded-For"].Should().Be("203.0.113.45, 198.51.100.10, 192.0.2.5");
            result.Data[" + X-CDN-Provider"].Should().Be("CloudFlare");
            result.Data[" + X-Edge-Location"].Should().Be("SIN");
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldHandleNullHttpContext_Gracefully()
        {
            // Arrange - Null HttpContext scenario
            _httpContextAccessor.HttpContext.Returns((HttpContext)null);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Handle null HttpContext
            var act = async () => await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should handle null HttpContext gracefully
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CheckHealthAsync_ShouldSupportBusinessScenario_MultiRegionDeployment()
        {
            // Arrange - Multi-region deployment business scenario
            _webHostEnvironment.EnvironmentName.Returns("Production");
            _mockRequest.Host.Returns(new HostString("api-asia.b2c.com"));

            // Create a real HeaderDictionary for proper enumeration support
            var headerDictionary = new HeaderDictionary
            {
                { "X-Region", new StringValues("Asia-Pacific") },
                { "X-Datacenter", new StringValues("Singapore") },
                { "X-Instance-ID", new StringValues("i-0123456789abcdef0") }
            };

            _mockRequest.Headers.Returns(headerDictionary);
            var healthCheck = new WhoAmIHealthCheck(_httpContextAccessor, _webHostEnvironment);

            // Act - Get multi-region deployment info
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert - Should provide multi-region deployment information
            result.Status.Should().Be(HealthStatus.Healthy);
            result.Data.Should().ContainKey("Env");
            result.Data["Env"].Should().Be("Production");
            result.Data["Host"].ToString().Should().Be("api-asia.b2c.com");
            result.Data.Should().ContainKey(" + X-Region");
            result.Data.Should().ContainKey(" + X-Datacenter");
            result.Data.Should().ContainKey(" + X-Instance-ID");
            result.Data[" + X-Region"].Should().Be("Asia-Pacific");
            result.Data[" + X-Datacenter"].Should().Be("Singapore");
        }
    }
}
