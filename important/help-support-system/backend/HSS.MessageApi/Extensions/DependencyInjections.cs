﻿using HSS.Common;
using HSS.SharedServices.Messages;
using HSS.SharedServices.Sql.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HSS.MessageApi.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Get values from the config given their key and their target type.
            IdentityPlatformSettings settings = configuration.GetRequiredSection("IdentityPlatformSettings").Get<IdentityPlatformSettings>();

            var connectionString = configuration["ConnectionStrings:DefaultConnection"]!;
            services.AddSingleton(settings); 
            services.AddScoped<IMessageService, MessageService>(_ => new MessageService(connectionString));
            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            // Add services to the container.
            var settings = services.BuildServiceProvider().GetService<IdentityPlatformSettings>()!;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Configure the Authority to the expected value for
                    // the authentication provider. This ensures the token
                    // is appropriately validated.
                    // automic validate
                    options.Audience = settings.Audience;
                    options.Authority = settings.Authority;
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine("EEEEEEEEEEEE" + ctx.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            Console.WriteLine("tokenvalidated" + ctx.Result);
                            return Task.CompletedTask;
                        },
                        OnForbidden = ctx =>
                        {
                            Console.WriteLine("Forbbiden my block reason " + ctx.Options.Audience);
                            return Task.CompletedTask;
                        }
                    };
                });
            return services;
        }
    }
}