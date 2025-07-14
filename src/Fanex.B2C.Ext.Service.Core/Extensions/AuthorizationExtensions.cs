using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.B2C.Ext.Service.Core.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler, FunctionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, LicenseeAuthorizationHandler>();

            return services;
        }

        public static AuthorizationOptions AddCustomPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy("AffInfo", policy =>
                policy.Requirements.Add(new FunctionRequirement(Function.AffInfo)));

            options.AddPolicy("Statistics", policy =>
                policy.Requirements.Add(new FunctionRequirement(Function.Statistics)));

            options.AddPolicy("Exchange", policy =>
                policy.Requirements.Add(new FunctionRequirement(Function.Exchange)));

            return options;
        }
    }
} 