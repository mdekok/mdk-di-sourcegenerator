﻿// <auto-generated />

using Microsoft.Extensions.DependencyInjection;

namespace Mdk.DISourceGenerator;

public static partial class DIRegistrations
{
    public static IServiceCollection RegisterServicesDevConsoleApp(this IServiceCollection services)
    {
        if (registeredServicesDevConsoleApp)
            return services;

        services.RegisterServicesReferencedLibrary();

        services.AddScoped<global::DevConsoleApp.Services.IInterface1, global::DevConsoleApp.Services.RegisterInterfaceMultipleTimes1>();
        services.AddScoped<global::DevConsoleApp.Services.IInterface1, global::DevConsoleApp.Services.RegisterInterfaceMultipleTimes2>();

        registeredServicesDevConsoleApp = true;

        return services;
    }

    private static bool registeredServicesDevConsoleApp;
}
