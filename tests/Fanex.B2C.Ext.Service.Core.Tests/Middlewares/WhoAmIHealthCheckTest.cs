using Fanex.B2C.Ext.Service.Core.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Middlewares
{
    public class WhoAmIHealthCheckTest
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IWebHostEnvironment WebHostEnvironment { get; set; }

        public WhoAmIHealthCheckTest()
        {
            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            WebHostEnvironment = Substitute.For<IWebHostEnvironment>();
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
            request.Body.Returns(new MemoryStream());

            // Setup connection
            connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse(clientIP));
            connection.LocalIpAddress.Returns(System.Net.IPAddress.Parse("127.0.0.1"));

            // Setup context
            context.Request.Returns(request);
            context.Connection.Returns(connection);

            // CRITICAL: Link request back to context for GetClientIP extension method
            request.HttpContext.Returns(context);

            return context;
        }

        [Fact]
        public async Task CheckHealthAsync_WhenCall_ReturnHealthy()
        {
            //Arrange
            var mockContext = CreateMockHttpContext();
            HttpContextAccessor.HttpContext.Returns(mockContext);
            WebHostEnvironment.EnvironmentName.Returns("Test");

            var healthCheck = new WhoAmIHealthCheck(HttpContextAccessor, WebHostEnvironment);

            //Action
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            //Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }
}