using Fanex.B2C.Ext.Service.Core.Helper;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Helpers
{
    public class ErrorMessageHelperTest
    {
        private readonly HttpContext _mockHttpContext;

        public ErrorMessageHelperTest()
        {
            _mockHttpContext = CreateMockHttpContext();
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
            connection.RemoteIpAddress.Returns(IPAddress.Parse(clientIP));

            // Setup context
            context.Request.Returns(request);
            context.Connection.Returns(connection);

            // CRITICAL: Link request back to context for GetClientIP extension method
            request.HttpContext.Returns(context);

            return context;
        }

        [Fact]
        public void GenerateMessage_WhenCallWithGeneralException_ReturnCorrectString()
        {
            //Arrange
            var exception = new Exception("test");

            //Action
            var result = ErrorMessageHelper.GenerateMessage(_mockHttpContext, exception);

            //Assert
            Assert.NotNull(result);
            Assert.Contains("Host", result);
            Assert.Contains("Path", result);
            Assert.Contains("Body", result);
            Assert.Contains("Remote IP", result);
        }

        [Fact]
        public void GenerateMessage_WhenCallWithExceptionHaveInnerException_ReturnStringHaveInnerExcetion()
        {
            //Arrange
            var innerException = new Exception("Inner");
            var exception = new Exception("Outer", innerException);

            //Action
            var result = ErrorMessageHelper.GenerateMessage(_mockHttpContext, exception);

            //Assert
            Assert.Contains("Outer", result);
            Assert.Contains("Inner", result);
            Assert.True(result.IndexOf("Inner") > result.IndexOf("Outer"));
        }
    }
}