namespace Fanex.B2C.Ext.Service.Core.Helper
{
    using Fanex.B2C.Ext.Service.Core.Extensions;
    using Fanex.Data;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    public static class ErrorMessageHelper
    {
        public static string GenerateMessage(HttpContext context, Exception exception)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"ErrMsg: {exception.Message}");
            AddRequestInfo(context, messageBuilder);
            AddDatabaseInfo(messageBuilder, exception);

            const int limitedNumOfInnerException = 3;
            AddExceptionInfo(messageBuilder, exception, limitedNumOfInnerException);
            return messageBuilder.ToString();
        }

        private static void AddRequestInfo(HttpContext context, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine("REQUEST INFO");

            var webInfo = GetRequestInfo(context);

            foreach (var item in webInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddDatabaseInfo(StringBuilder builder, Exception exception)
        {
            builder.AppendLine();
            builder.AppendLine("DATABASE INFO");

            var dbInfo = GetDbExceptionInfo(exception);

            foreach (var item in dbInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddExceptionInfo(StringBuilder builder, Exception exception,
            int limitedInnerException)
        {
            builder.AppendLine();
            builder.AppendLine("EXCEPTION INFO");
            builder.AppendLine($"Message: {exception.Message}");
            builder.AppendLine($"Source: {exception.Source}");
            builder.AppendLine($"TargetSite: {exception.TargetSite}");
            builder.AppendLine($"StackTrace: {exception.StackTrace}");
            if (exception.InnerException != null && limitedInnerException > 0)
            {
                limitedInnerException--;
                AddExceptionInfo(builder, exception.InnerException, limitedInnerException);
            }
        }

        private static Dictionary<string, object> GetDbExceptionInfo(Exception exception)
        {
            var dbInfo = new Dictionary<string, object>();
            BindSqlExceptionInfo(exception, dbInfo);
            return dbInfo;
        }

        private static void BindSqlExceptionInfo(Exception exception, IDictionary<string, object> dbInfo)
        {
            if (exception is SqlException sqlException)
            {
                dbInfo["Server"] = sqlException.Server;
                dbInfo["SpName"] = sqlException.Procedure;
                dbInfo["Line"] = sqlException.LineNumber;
            }
        }

        private static Dictionary<string, object> GetRequestInfo(HttpContext context)
        {
            var request = context.Request;

            return new Dictionary<string, object>
            {
                [nameof(request.Host)] = request.Host,
                [nameof(request.Path)] = request.Path,
                [nameof(request.Body)] = request.Body,
                ["Remote IP"] = context.Connection.RemoteIpAddress,
                ["Client IP"] = context.Request.GetClientIP(),
            };
        }
    }
}
