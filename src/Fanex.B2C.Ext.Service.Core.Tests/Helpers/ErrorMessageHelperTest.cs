using System.Data.SqlClient;
using Fanex.B2C.Ext.Service.Core.Helper;
using Fanex.Data;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Helpers
{
    public class ErrorMessageHelperTest
    {
        private readonly HttpContext _mockHttpContext;

        public ErrorMessageHelperTest()
        {
            _mockHttpContext = Substitute.For<HttpContext>();
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
        public void GenerateMessage_WhenCallWithSqlException_ReturnStringHaveSqlExceptionInfo()
        {
            //Arrange
            var connectionString = "Data Source=127.0.0.1;Initial Catalog=testDB; integrated security=false; User Id=test; Password=test";
            SqlException sqlException = null;
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    sqlException = ex;
                }
            }

            //Action
            var result = ErrorMessageHelper.GenerateMessage(_mockHttpContext, sqlException);

            //Assert
            Assert.NotNull(sqlException);
            Assert.Contains("Server", result);
            Assert.Contains("SpName", result);
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

        [Fact]
        public void GenerateMessage_WhenCallWithObjectDbException_ReturnStringHaveObjectDBExcptionInfo()
        {
            //Arrange
            var ex = new ObjectDbException("test", new Exception());

            //Action
            var result = ErrorMessageHelper.GenerateMessage(_mockHttpContext, ex);

            //Assert
            Assert.Contains("Server", result);
            Assert.Contains("DbName", result);
            Assert.Contains("SpName", result);
            Assert.Contains("UserId", result);
            Assert.Contains("Parameters", result);
            Assert.Contains("CommandTimeout", result);
        }
    }
}