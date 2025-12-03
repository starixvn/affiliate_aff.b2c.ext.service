using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Fanex.B2C.Ext.Service.Core.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UsePingCheck(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    return;
                }
                await next();
            });
        }

        public static IApplicationBuilder UseJWTokenOnPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandler>();
        }

        public static void UseStaticResource(this IApplicationBuilder app, bool isDev)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int OneYear = 60 * 60 * 24 * 365;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = isDev ?
                    "no-cache" : "public,max-age=" + OneYear;
                }
            });
        }

        public static void UseUnderMaintenance(this IApplicationBuilder app)
        {
            app.UseMiddleware<UnderMaintenanceHandler>();
        }

        public static void UseWhoAmICheck(this IApplicationBuilder app)
        {
            Func<HttpContext, HealthReport, Task> customResponseWriter = async (context, report) =>
            {
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        status = report.Status.ToString(),
                        monitors = report.Entries.Select(e => new { name = e.Key, status = Enum.GetName(typeof(HealthStatus), e.Value.Status), values = e.Value.Data })
                    }, Formatting.Indented);

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            };

            app.UseHealthChecks("/healthz", options: new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = _ => true,
                ResponseWriter = customResponseWriter
            });
        }
    }
}