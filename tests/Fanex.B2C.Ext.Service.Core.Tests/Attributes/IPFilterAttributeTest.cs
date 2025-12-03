using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Attributes
{
    public class IPFilterAttributeTest
    {
        private readonly ILogger<IPFilterAttribute> _logger;
        private readonly IOptions<ApplicationOptions> _options;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly HttpContext _mockHttpContext;

        public IPFilterAttributeTest()
        {
            _mockHttpContext = CreateMockHttpContext();
            _logger = Substitute.For<ILogger<IPFilterAttribute>>();
            _options = Substitute.For<IOptions<ApplicationOptions>>();

            var actionContext = new ActionContext(
                _mockHttpContext,
                Substitute.For<RouteData>(),
                Substitute.For<ActionDescriptor>(),
                new ModelStateDictionary()
            );

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );

            // HttpRequest is already properly set up in CreateMockHttpContext
            // No need to override it here
        }

        [Fact]
        public void OnActionExecuting_WhenFromInternal_NoChangeResult()
        {
            //Arrange
            var fakeContent = "No_Change";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string>() });

            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Host.Returns(HostString.FromUriComponent(""));

            _mockHttpContext.Request.Returns(httpRequest);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }


        [Fact]
        public void OnActionExecuting_WhenEmptyWhiteListIP_NoChangeResult()
        {
            //Arrange
            var fakeContent = "No_Change";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string>() });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(0x2414188f));
            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_WhenAllowIP_WhiteListSingle_NoChangeResult()
        {
            //Arrange
            const int requestIP = 0x2414188f;
            const string WhitelistIP = "143.24.20.36";

            const string fakeContent = "NOCHANGE";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(requestIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_WhenNotAllowIP_WhiteListSingle_ReturnResultWithDeniedMessage()
        {
            //Arrange
            const long requestIPNumber = 1532995706;
            const string requestIP = "122.168.95.91";
            const string WhitelistIP = "172.168.95.121";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(new IPAddress(requestIPNumber));

            _mockHttpContext.Connection.Returns(connectionInfo);


            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.121")]
        [InlineData("122.168.95.122")]
        [InlineData("122.168.95.123")]
        [InlineData("122.168.95.124")]
        [InlineData("122.168.95.125")]
        [InlineData("122.168.95.126")]
        [InlineData("122.168.95.127")]
        public void OnActionExecuting_WhenAllowIP_WhiteListRange_NoChangeResult(string requestIP)
        {
            //Arrange
            const string WhitelistIP = "122.168.95.121/29";

            const string fakeContent = "NOCHANGE";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));
            _mockHttpContext.Connection.Returns(connectionInfo);
            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.119")]
        [InlineData("122.168.95.128")]
        public void OnActionExecuting_WhenNotAllowIP_WhiteListRange_ReturnResultWithDeniedMessage(string requestIP)
        {
            //Arrange
            const string WhitelistIP = "122.168.95.121/29";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { WhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));

            _mockHttpContext.Connection.Returns(connectionInfo);


            //Action
            var filter = new IPFilterAttribute(_logger, _options);

            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
        }

        [Theory]
        [InlineData("122.168.95.256")]
        [InlineData("122.168.95.121/100")]
        public void OnActionExecuting_WhiteListIPInvalid_ThrowExceptionWithMessage(string whiteListIP)
        {
            //Arrange
            const string requestIP = "122.168.95.121";

            var expectedResult = $"The IP Address: {requestIP} was blocked";
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { whiteListIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(requestIP));

            _mockHttpContext.Connection.Returns(connectionInfo);

            //Action
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            //Assert
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            Assert.Equal(expectedResult, actualResult.Content);
            _logger.Received().LogError($"The WhiteList IP: {whiteListIP} was invalid - client IP: ${requestIP}");
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_InternalAPIAccess()
        {
            // Arrange - Internal API access business scenario
            var fakeContent = "Internal_Access_Allowed";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { "192.168.1.0/24" } });

            // Setup internal request (using intb2c in host)
            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Host.Returns(new HostString("api.intb2c.local"));
            _mockHttpContext.Request.Returns(httpRequest);

            // Act - Process internal API request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow internal access without IP checking
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_ExternalPartnerAccess()
        {
            // Arrange - External partner access business scenario
            const string partnerIP = "203.0.113.45";
            const string fakeContent = "Partner_Access_Allowed";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            // Setup whitelist with partner IP range
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { "203.0.113.0/24" } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(partnerIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process partner API request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow partner access from whitelisted IP
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_UnauthorizedAccessBlocking()
        {
            // Arrange - Unauthorized access blocking business scenario
            const string unauthorizedIP = "198.51.100.45";
            const string whitelistIP = "203.0.113.0/24";

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { whitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(unauthorizedIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process unauthorized API request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should block unauthorized access with clear message
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal($"The IP Address: {unauthorizedIP} was blocked", actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_MultiplePartnerWhitelisting()
        {
            // Arrange - Multiple partner whitelisting business scenario
            const string partner1IP = "203.0.113.45";
            const string fakeContent = "Multi_Partner_Access";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            // Setup whitelist with multiple partner ranges
            _options.Value.Returns(new ApplicationOptions
            {
                WhiteList = new List<string>
                {
                    "203.0.113.0/24",  // Partner 1
                    "198.51.100.0/24", // Partner 2
                    "192.0.2.0/24"     // Partner 3
                }
            });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(partner1IP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process multi-partner API request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow access from any whitelisted partner
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_InvalidWhitelistConfiguration()
        {
            // Arrange - Invalid whitelist configuration business scenario
            const string clientIP = "192.168.1.100";
            const string invalidWhitelistIP = "300.300.300.300"; // Invalid IP

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { invalidWhitelistIP } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(clientIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process request with invalid whitelist configuration
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should block access and log error for invalid configuration
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal($"The IP Address: {clientIP} was blocked", actualResult.Content);

            // Should log error about invalid whitelist IP
            _logger.Received().LogError($"The WhiteList IP: {invalidWhitelistIP} was invalid - client IP: ${clientIP}");
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_EmptyWhitelistAllowsAll()
        {
            // Arrange - Empty whitelist allows all business scenario
            const string anyIP = "203.0.113.45";
            const string fakeContent = "Empty_Whitelist_Allows_All";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            // Setup empty whitelist
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string>() });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(anyIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process request with empty whitelist
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow access when whitelist is empty
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Theory]
        [InlineData("192.168.1.0/24", "192.168.1.50", true)]
        [InlineData("10.0.0.0/8", "10.5.10.15", true)]
        [InlineData("172.16.0.0/12", "172.20.5.100", true)]
        [InlineData("192.168.1.0/24", "192.168.2.50", false)]
        [InlineData("10.0.0.0/16", "11.0.0.1", false)]
        public void OnActionExecuting_ShouldHandleIPRangeValidation_Correctly(string whitelistRange, string clientIP, bool shouldAllow)
        {
            // Arrange - IP range validation scenario
            const string fakeContent = "Range_Validation_Test";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { whitelistRange } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(clientIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process request with IP range validation
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should handle IP range validation correctly
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);

            if (shouldAllow)
            {
                Assert.Equal(fakeContent, actualResult.Content);
            }
            else
            {
                Assert.Equal($"The IP Address: {clientIP} was blocked", actualResult.Content);
            }
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_LoadBalancerIPHandling()
        {
            // Arrange - Load balancer IP handling business scenario
            const string loadBalancerIP = "10.0.0.100";
            const string fakeContent = "LoadBalancer_Access";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            // Setup whitelist with load balancer IP range
            _options.Value.Returns(new ApplicationOptions { WhiteList = new List<string> { "10.0.0.0/24" } });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(loadBalancerIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process load balancer request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow load balancer access
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        [Fact]
        public void OnActionExecuting_ShouldSupportBusinessScenario_CDNProviderAccess()
        {
            // Arrange - CDN provider access business scenario
            const string cdnIP = "203.0.113.25";
            const string fakeContent = "CDN_Access_Allowed";
            var fakeResult = new ContentResult { Content = fakeContent };
            _actionExecutingContext.Result = fakeResult;

            // Setup whitelist with CDN IP ranges
            _options.Value.Returns(new ApplicationOptions
            {
                WhiteList = new List<string>
                {
                    "203.0.113.0/24",  // CDN Range 1
                    "198.51.100.0/24"  // CDN Range 2
                }
            });

            var connectionInfo = Substitute.For<ConnectionInfo>();
            connectionInfo.RemoteIpAddress.Returns(IPAddress.Parse(cdnIP));
            _mockHttpContext.Connection.Returns(connectionInfo);

            // Act - Process CDN request
            var filter = new IPFilterAttribute(_logger, _options);
            filter.OnActionExecuting(_actionExecutingContext);

            // Assert - Should allow CDN provider access
            var actualResult = Assert.IsAssignableFrom<ContentResult>(_actionExecutingContext.Result);
            Assert.Equal(fakeContent, actualResult.Content);
        }

        private static HttpContext CreateMockHttpContext(string host = "localhost", string path = "/", string method = "GET", string clientIP = "127.0.0.1")
        {
            var context = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();
            var connection = Substitute.For<ConnectionInfo>();

            // Setup request
            request.Host.Returns(new HostString(host));
            request.Path.Returns(new PathString(path));
            request.Method.Returns(method);

            // Setup connection
            connection.RemoteIpAddress.Returns(IPAddress.Parse(clientIP));

            // Setup context
            context.Request.Returns(request);
            context.Connection.Returns(connection);

            // CRITICAL: Link request back to context for GetClientIP extension method
            request.HttpContext.Returns(context);

            return context;
        }
    }
}