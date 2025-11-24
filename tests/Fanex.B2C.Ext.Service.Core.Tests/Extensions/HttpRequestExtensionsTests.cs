using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Extensions
{
    /// <summary>
    /// Comprehensive unit tests for HttpRequestExtensions.
    /// Tests HTTP request processing scenarios and client IP detection business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void FromInternal_ShouldReturnTrue_WhenHostContainsInternalDomain()
        {
            // Arrange - Internal domain scenario
            var request = CreateMockRequest("test.intb2c.com");

            // Act - Check if request is from internal domain
            var result = request.FromInternal();

            // Assert - Should return true for internal domain
            result.Should().BeTrue();
        }

        [Fact]
        public void FromInternal_ShouldReturnTrue_WhenHostMatchesSystemHostName()
        {
            // Arrange - System hostname scenario
            var systemHostName = Dns.GetHostName();
            var request = CreateMockRequest(systemHostName);

            // Act - Check if request is from system hostname
            var result = request.FromInternal();

            // Assert - Should return true for system hostname
            result.Should().BeTrue();
        }

        [Fact]
        public void FromInternal_ShouldReturnFalse_WhenHostIsExternal()
        {
            // Arrange - External domain scenario
            var request = CreateMockRequest("external.example.com");

            // Act - Check if request is from external domain
            var result = request.FromInternal();

            // Assert - Should return false for external domain
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("api.intb2c.com")]
        [InlineData("test.intb2c.internal")]
        [InlineData("dev-intb2c.local")]
        public void FromInternal_ShouldReturnTrue_WhenHostContainsIntB2cDomain(string hostName)
        {
            // Arrange - Various internal domain scenarios
            var request = CreateMockRequest(hostName);

            // Act - Check if request is from internal domain variations
            var result = request.FromInternal();

            // Assert - Should return true for various internal domain patterns
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("google.com")]
        [InlineData("microsoft.com")]
        [InlineData("external-api.com")]
        [InlineData("malicious-site.net")]
        public void FromInternal_ShouldReturnFalse_WhenHostIsExternalDomain(string hostName)
        {
            // Arrange - Various external domain scenarios
            var request = CreateMockRequest(hostName);

            // Act - Check if request is from external domains
            var result = request.FromInternal();

            // Assert - Should return false for external domains
            result.Should().BeFalse();
        }

        [Fact]
        public void GetClientIP_ShouldReturnActualIP_WhenXActualIPHeaderExists()
        {
            // Arrange - X-Actual-IP header scenario
            var expectedIP = "192.168.1.100";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_ACTUAL_IP] = expectedIP
            });

            // Act - Get client IP with X-Actual-IP header
            var result = request.GetClientIP();

            // Assert - Should return X-Actual-IP header value
            result.Should().Be(expectedIP);
        }

        [Fact]
        public void GetClientIP_ShouldReturnForwardedIP_WhenXForwardedForHeaderExists()
        {
            // Arrange - X-Forwarded-For header scenario
            var expectedIP = "203.0.113.45";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = expectedIP
            });

            // Act - Get client IP with X-Forwarded-For header
            var result = request.GetClientIP();

            // Assert - Should return X-Forwarded-For header value
            result.Should().Be(expectedIP);
        }

        [Fact]
        public void GetClientIP_ShouldPrioritizeActualIP_WhenBothHeadersExist()
        {
            // Arrange - Both headers exist scenario
            var actualIP = "192.168.1.100";
            var forwardedIP = "203.0.113.45";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_ACTUAL_IP] = actualIP,
                [HeaderKey.X_FORWARDED_FOR] = forwardedIP
            });

            // Act - Get client IP when both headers exist
            var result = request.GetClientIP();

            // Assert - Should prioritize X-Actual-IP over X-Forwarded-For
            result.Should().Be(actualIP);
        }

        [Fact]
        public void GetClientIP_ShouldReturnRemoteIP_WhenNoHeadersExist()
        {
            // Arrange - No headers scenario
            var remoteIP = IPAddress.Parse("10.0.0.1");
            var request = CreateMockRequestWithRemoteIP(remoteIP);

            // Act - Get client IP with no headers
            var result = request.GetClientIP();

            // Assert - Should return remote IP address
            result.Should().Be("10.0.0.1");
        }

        [Fact]
        public void GetClientIP_ShouldReturnLocalhost_WhenRemoteIPIsIPv6Localhost()
        {
            // Arrange - IPv6 localhost scenario
            var remoteIP = IPAddress.IPv6Loopback; // ::1
            var request = CreateMockRequestWithRemoteIP(remoteIP);

            // Act - Get client IP for IPv6 localhost
            var result = request.GetClientIP();

            // Assert - Should convert IPv6 localhost to IPv4 localhost
            result.Should().Be("127.0.0.1");
        }

        [Theory]
        [InlineData("192.168.1.1, 203.0.113.45, invalid")]
        [InlineData("203.0.113.45, 192.168.1.1")]
        [InlineData("invalid, 192.168.1.100")]
        public void GetClientIP_ShouldReturnFirstValidIP_WhenMultipleIPsInHeader(string ipList)
        {
            // Arrange - Multiple IPs in header scenario
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = ipList
            });

            // Act - Get client IP from multiple IPs
            var result = request.GetClientIP();

            // Assert - Should return first valid IP from the list
            result.Should().MatchRegex(@"^(\d{1,3}\.){3}\d{1,3}$"); // Valid IPv4 format

            // Should be one of the valid IPs from the list
            var validIPs = new[] { "192.168.1.1", "203.0.113.45", "192.168.1.100" };
            validIPs.Should().Contain(result);
        }

        [Fact]
        public void GetClientIP_ShouldHandleInvalidIPsGracefully_WhenHeaderContainsInvalidData()
        {
            // Arrange - Invalid IP data scenario
            var remoteIP = IPAddress.Parse("10.0.0.1");
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = "invalid-ip, not-an-ip, 999.999.999.999"
            }, remoteIP);

            // Act - Get client IP with invalid header data
            var result = request.GetClientIP();

            // Assert - Should fall back to remote IP when header contains invalid data
            result.Should().Be("10.0.0.1");
        }

        [Fact]
        public void GetClientIP_ShouldSupportBusinessScenario_LoadBalancerConfiguration()
        {
            // Arrange - Load balancer configuration business scenario
            var clientIP = "203.0.113.100";
            var loadBalancerIP = "192.168.1.10";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = $"{clientIP}, {loadBalancerIP}"
            });

            // Act - Get client IP from load balancer setup
            var result = request.GetClientIP();

            // Assert - Should return actual client IP, not load balancer IP
            result.Should().Be(clientIP);
        }

        [Fact]
        public void GetClientIP_ShouldSupportBusinessScenario_ProxyChainHandling()
        {
            // Arrange - Proxy chain handling business scenario
            var originalClientIP = "203.0.113.200";
            var proxy1IP = "192.168.1.50";
            var proxy2IP = "192.168.1.51";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = $"{originalClientIP}, {proxy1IP}, {proxy2IP}"
            });

            // Act - Get client IP from proxy chain
            var result = request.GetClientIP();

            // Assert - Should return original client IP from proxy chain
            result.Should().Be(originalClientIP);
        }

        [Fact]
        public void GetClientIP_ShouldSupportBusinessScenario_CDNConfiguration()
        {
            // Arrange - CDN configuration business scenario
            var realClientIP = "198.51.100.50";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_ACTUAL_IP] = realClientIP,
                [HeaderKey.X_FORWARDED_FOR] = "192.168.1.1" // CDN internal IP
            });

            // Act - Get client IP from CDN setup
            var result = request.GetClientIP();

            // Assert - Should prioritize actual IP over forwarded IP in CDN setup
            result.Should().Be(realClientIP);
        }

        [Fact]
        public void GetClientIP_ShouldSupportBusinessScenario_SecurityAuditLogging()
        {
            // Arrange - Security audit logging business scenario
            var suspiciousIP = "198.51.100.100";
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_FORWARDED_FOR] = suspiciousIP
            });

            // Act - Get client IP for security audit
            var clientIP = request.GetClientIP();

            // Assert - Should provide accurate client IP for security audit
            clientIP.Should().Be(suspiciousIP);
            clientIP.Should().MatchRegex(@"^(\d{1,3}\.){3}\d{1,3}$"); // Valid IPv4 for audit logs
        }

        [Fact]
        public void GetClientIP_ShouldSupportBusinessScenario_GeolocationServices()
        {
            // Arrange - Geolocation services business scenario
            var geoLocationIP = "8.8.8.8"; // Public IP for geolocation
            var request = CreateMockRequestWithHeaders(new Dictionary<string, StringValues>
            {
                [HeaderKey.X_ACTUAL_IP] = geoLocationIP
            });

            // Act - Get client IP for geolocation services
            var result = request.GetClientIP();

            // Assert - Should provide accurate IP for geolocation services
            result.Should().Be(geoLocationIP);
            result.Should().NotBe("127.0.0.1"); // Should not be localhost for geolocation
            result.Should().NotStartWith("192.168."); // Should not be private IP for geolocation
        }

        [Fact]
        public void FromInternal_ShouldSupportBusinessScenario_InternalAPIAccess()
        {
            // Arrange - Internal API access business scenario
            var internalAPIRequest = CreateMockRequest("api.intb2c.internal");

            // Act - Check if request is from internal API
            var isInternal = internalAPIRequest.FromInternal();

            // Assert - Should allow internal API access
            isInternal.Should().BeTrue();
        }

        [Fact]
        public void FromInternal_ShouldSupportBusinessScenario_ExternalAPIRestriction()
        {
            // Arrange - External API restriction business scenario
            var externalRequest = CreateMockRequest("malicious-external.com");

            // Act - Check if request is from external source
            var isInternal = externalRequest.FromInternal();

            // Assert - Should restrict external API access
            isInternal.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void GetClientIP_ShouldHandleEmptyHeaders_Gracefully(string headerValue)
        {
            // Arrange - Empty headers scenario
            var remoteIP = IPAddress.Parse("10.0.0.2");
            var headers = new Dictionary<string, StringValues>();

            if (headerValue != null)
            {
                headers[HeaderKey.X_FORWARDED_FOR] = headerValue;
            }

            var request = CreateMockRequestWithHeaders(headers, remoteIP);

            // Act - Get client IP with empty headers
            var result = request.GetClientIP();

            // Assert - Should fall back to remote IP
            result.Should().Be("10.0.0.2");
        }

        private static HttpRequest CreateMockRequest(string hostName)
        {
            var request = Substitute.For<HttpRequest>();
            request.Host.Returns(new HostString(hostName));
            return request;
        }

        private static HttpRequest CreateMockRequestWithHeaders(Dictionary<string, StringValues> headers, IPAddress remoteIP = null)
        {
            var request = Substitute.For<HttpRequest>();
            var headerDictionary = Substitute.For<IHeaderDictionary>();
            var httpContext = Substitute.For<HttpContext>();
            var connection = Substitute.For<ConnectionInfo>();

            // Setup headers
            foreach (var header in headers)
            {
                headerDictionary.TryGetValue(header.Key, out Arg.Any<StringValues>()).Returns(x =>
                {
                    x[1] = header.Value;
                    return true;
                });
            }

            // Setup connection
            connection.RemoteIpAddress.Returns(remoteIP ?? IPAddress.Parse("127.0.0.1"));
            httpContext.Connection.Returns(connection);

            request.Headers.Returns(headerDictionary);
            request.HttpContext.Returns(httpContext);

            return request;
        }

        private static HttpRequest CreateMockRequestWithRemoteIP(IPAddress remoteIP)
        {
            var request = Substitute.For<HttpRequest>();
            var headerDictionary = Substitute.For<IHeaderDictionary>();
            var httpContext = Substitute.For<HttpContext>();
            var connection = Substitute.For<ConnectionInfo>();

            // Setup empty headers
            headerDictionary.TryGetValue(Arg.Any<string>(), out Arg.Any<StringValues>()).Returns(false);

            // Setup connection with remote IP
            connection.RemoteIpAddress.Returns(remoteIP);
            httpContext.Connection.Returns(connection);

            request.Headers.Returns(headerDictionary);
            request.HttpContext.Returns(httpContext);

            return request;
        }
    }
}
