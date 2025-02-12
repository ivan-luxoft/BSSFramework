﻿using System;

using DotNetCore.CAP;

using Framework.Authorization.BLL;
using Framework.Cap.Abstractions;
using Framework.Cap.Auth;
using Framework.Cap.Impl;
using Framework.DomainDriven.BLL.Security;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

using Savorboard.CAP.InMemoryMessageQueue;

namespace Framework.Cap;

public static class DependencyInjections
{
    public const string CapAuthenticationScheme = "CapAuthenticationScheme";

    /// <summary>
    /// Add CAP with authentication. For authentication add this code
    /// <code>
    ///     services.AddAuthentication().AddCapAuth&lt;ISampleSystemBLLContext&gt;();
    /// </code>
    /// </summary>
    public static IServiceCollection AddCapBss(
            this IServiceCollection services,
            string connectionString,
            Action<CapOptions> setupAction = null)
    {
        services
                .AddSingleton<ICapTransactionManager, CapTransactionManager>()
                .AddScoped<IIntegrationEventBus, IntegrationEventBus>()
                .AddCap(
                        z =>
                        {
                            z.FailedRetryCount = 2;
                            z.UseSqlServer(
                                           x =>
                                           {
                                               x.ConnectionString = connectionString;
                                               x.Schema = "bus";
                                           });
                            z.UseInMemoryMessageQueue();

                            z.UseDashboard(
                                           x =>
                                           {
                                               x.UseAuth = true;
                                               x.DefaultAuthenticationScheme = CapAuthenticationScheme;
                                           });

                            setupAction?.Invoke(z);
                        });

        return services;
    }

    /// <summary>
    /// Add CAP authentication (User with role Administrator has access only)
    /// </summary>
    public static AuthenticationBuilder AddCapAuth<TBllContext>(
            this AuthenticationBuilder builder,
            Action<AuthenticationSchemeOptions> configureOptions = null)
            where TBllContext : IAuthorizationBLLContextContainer<IAuthorizationBLLContext> =>
            builder.AddScheme<AuthenticationSchemeOptions, CapAuthenticationHandler<TBllContext>>(
             CapAuthenticationScheme,
             CapAuthenticationScheme,
             configureOptions);
}
