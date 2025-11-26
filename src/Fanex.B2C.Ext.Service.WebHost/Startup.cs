using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Extensions;
using Fanex.B2C.Ext.Service.Core.Middleware;
using Fanex.B2C.Ext.Service.Core.Models;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ONEbook.UM.DependencyInjection;
using ONEbook.UM.Services;
using Sentry.AspNetCore;
using System.Security.Claims;
using System.Text;

namespace Fanex.B2C.Ext.Service.WebHost
{
    public class Startup(IConfiguration configuration)
    {
        private const string ApplicationOptionSettingKey = "ApplicationOptions";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aff.B2C.Ext.Service", Version = "v1" });
            });

            services.Configure<SentryAspNetCoreOptions>(configuration.GetSection("Sentry"));
            services.Configure<TokenManagement>(configuration.GetSection("tokenManagement"));
            services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptionSettingKey));

            services.Configure<List<User>>(configuration.GetSection("Users"));

            services.AddFanexData(configuration);

            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
            });

            ConfigAuthentication(services);

            services.AddAuthorization(options =>
            {
                // Set default policy
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                // Add custom policies
                options.AddCustomPolicies();
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHealthChecks()
                .AddCheck<WhoAmIHealthCheck>("WhoAmIHealthCheck");

            services.AddHttpContextAccessor();
            services.AddHttpsRedirection(options =>
            {
                const int httpsPort = 443;
                options.HttpsPort = httpsPort;
            });

            DIRegister(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePingCheck();
            app.UseForwardedHeaders();
            
            app.UseWhoAmICheck();
            
            if (string.Equals(configuration["UseSwagger"], "1"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (string.Equals(configuration["UseErrorHandler"], "1"))
            {
                app.UseErrorHandler();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.IsDevelopment())
            {
                app.UseHsts();

                app.UseWhen(
                    context => !context.Request.FromInternal(),
                    builder =>
                    {
                        builder.UseHttpsRedirection();
                    });
            }

            app.UseUnderMaintenance();
            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseJWTokenOnPage();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
        }

        private void DIRegister(IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddScoped<IAffililateInfoService, AffililateInfoService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IPFilterAttribute>();
            services.AddCustomAuthorization();

            services.AddOneBookUmForWeb(configuration);
        }

        private void ConfigAuthentication(IServiceCollection services)
        {
            var token = configuration.GetSection("tokenManagement").Get<TokenManagement>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RoleClaimType = ClaimsIdentity.DefaultNameClaimType,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}