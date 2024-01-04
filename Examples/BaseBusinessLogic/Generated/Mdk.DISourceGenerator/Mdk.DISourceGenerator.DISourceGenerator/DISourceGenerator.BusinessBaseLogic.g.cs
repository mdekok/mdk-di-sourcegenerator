﻿// <auto-generated />

using Microsoft.Extensions.DependencyInjection;

namespace Mdk.DISourceGenerator;

public static partial class DIRegistrations
{
    public static IServiceCollection RegisterServicesBusinessBaseLogic(this IServiceCollection services)
    {
        if (registeredServicesBusinessBaseLogic)
            return services;

        services.AddScoped<global::BusinessBaseLogic.IBaseInterface, global::BusinessBaseLogic.MyInterfacedBaseService>();
        services.AddScoped<global::BusinessBaseLogic.MyBaseService>();

        registeredServicesBusinessBaseLogic = true;

        return services;
    }

    private static bool registeredServicesBusinessBaseLogic;
}
