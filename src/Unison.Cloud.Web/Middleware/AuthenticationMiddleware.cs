using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Services;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Web.Setup
{
    public static class AuthenticationMiddleware
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            byte[] key = GetSecretKey(services, configuration);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();
                            var accountId = int.Parse(context.Principal.Identity.Name);
                            var account = accountService.Find(accountId);
                            if (account == null)
                            {
                                context.Fail("Unauthorized");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        private static byte[] GetSecretKey(IServiceCollection services, IConfiguration configuration)
        {
            var authConfigurationSection = configuration.GetSection("Authentication");
            services.Configure<AuthConfiguration>(authConfigurationSection);

            var authConfiguration = authConfigurationSection.Get<AuthConfiguration>();
            return Encoding.ASCII.GetBytes(authConfiguration.Secret);
        }
    }
}
