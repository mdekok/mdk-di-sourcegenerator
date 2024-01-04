﻿// <auto-generated />

using Microsoft.Extensions.DependencyInjection;

namespace Mdk.DISourceGenerator;

public static partial class DIRegistrations
{
    public static IServiceCollection RegisterServicesBlazorApp(this IServiceCollection services)
    {
        if (registeredServicesBlazorApp)
            return services;

        services.RegisterServicesBusinessBaseLogic();
        services.RegisterServicesBusinessLogic();

        registeredServicesBlazorApp = true;

        return services;
    }

    private static bool registeredServicesBlazorApp;
}
