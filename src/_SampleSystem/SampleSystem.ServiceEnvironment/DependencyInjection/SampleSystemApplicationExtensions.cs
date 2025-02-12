﻿using System;
using System.Reflection;

using Framework.Authorization.ApproveWorkflow;
using Framework.Authorization.BLL;
using Framework.Cap;
using Framework.DependencyInjection;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SampleSystem.BLL;
using SampleSystem.BLL.Core.Jobs;
using SampleSystem.BLL.Jobs;

namespace SampleSystem.ServiceEnvironment;

public static class SampleSystemApplicationExtensions
{
    public static IServiceCollection RegisterGeneralApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMediatR(Assembly.GetAssembly(typeof(EmployeeBLL)))
                       .RegisterSmtpNotification(configuration)
                       .RegisterWorkflowCore(configuration)
                       .RegisterApplicationServices()
                       .AddCapBss(configuration.GetConnectionString("DefaultConnection"))
                       .RegisterJobs();
    }

    private static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IExampleServiceForRepository, ExampleServiceForRepository>();

        services.ReplaceScoped<IAuthorizationValidator, SampleSystemCustomAuthValidator>();

        return services;
    }

    private static IServiceCollection RegisterSmtpNotification(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterNotificationJob<ISampleSystemBLLContext>();
        services.RegisterNotificationSmtp(configuration);
        services.RegisterRewriteReceiversDependencies(configuration);

        return services;
    }

    private static IServiceCollection RegisterWorkflowCore(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WorkflowCoreConnection");

        services.RegisterPureWorkflowCore(connectionString);

        services.AddSingleton<WorkflowManager>();
        services.AddSingletonFrom<IWorkflowManager, WorkflowManager>();

        services.AddAuthWorkflow();

        return services;
    }

    public static IServiceCollection RegisterPureWorkflowCore(this IServiceCollection services, string connectionString)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        return services
               .AddWorkflow(x => x.UseSqlServer(connectionString, true, true))
               .AddLogging();
    }

    private static IServiceCollection RegisterJobs(this IServiceCollection services)
    {
        services.AddScoped<ISampleJob, SampleJob>();
        services.AddScoped<StartWorkflowJob>();

        return services;
    }
}
