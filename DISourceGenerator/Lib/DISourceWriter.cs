﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DI source writer for combining all registration method calls and registration calls to referenced assemblies.</summary>
internal static class DISourceWriter
{
    /// <summary>Generates source code for the dependency injection registration.</summary>
    /// <param name="assemblyName">The assembly name, will be part of the registration method name.</param>
    /// <param name="registrations">The registrations to build sourcecode for.</param>
    /// <param name="referencedDIAssemblies">The referenced assemblies with additional generated registrations.</param>
    /// <returns>The dependency injection source code.</returns>
    internal static string Write(string assemblyName, List<DIRegistration> registrations, IEnumerable<IAssemblySymbol> referencedDIAssemblies)
    {
        IEnumerable<string?> registrationMethodCalls = BuildRegistrationMethodCalls(registrations);
        IEnumerable<string> assemblyMethodCalls = BuildReferencedDIAssembliesMethodCalls(referencedDIAssemblies);

        return MergeRegistrationSourceCode(assemblyName, registrationMethodCalls, assemblyMethodCalls);
    }

    /// <summary>Build method calls for registrations sorted on service lifetime and name.</summary>
    /// <param name="registrations">The registrations to build method calls for.</param>
    /// <returns>A list of string? containing registrations. Null implies divider between different lifetimes.</returns>
    private static IEnumerable<string?> BuildRegistrationMethodCalls(List<DIRegistration> registrations)
    {
        List<string?> methodCalls = [];
        string? lastMethod = null;
        foreach (DIRegistration registration in registrations
            .OrderBy(registration => registration.Method)
            .ThenBy(registration => registration.ServiceType.ToSource()))
        {
            if (lastMethod != registration.Method)
            {
                if (lastMethod is not null)
                {
                    // Add a blank line between different lifetimes.
                    methodCalls.Add(null);
                }
                lastMethod = registration.Method;
            }

            methodCalls.Add(registration.ToSource());

        }
        return methodCalls;
    }

    /// <summary>Build method calls for registering references assemblies.</summary>
    /// <param name="referencedDIAssemblies">The referenced assemblies with dependency registrations to build method call for.</param>
    /// <returns>A list of strings containing assembly registration method calls.</returns>
    private static IEnumerable<string> BuildReferencedDIAssembliesMethodCalls(IEnumerable<IAssemblySymbol> referencedDIAssemblies)
        => referencedDIAssemblies
            .OrderBy(assembly => assembly.Name)
            .Select(assembly => $"services.RegisterServices{Sanitize(assembly.Name)}()");

    /// <summary>Merges the dependency injection source code into the boiler plate source code.</summary>
    /// <param name="assemblyName">The assembly name, will be part of the registration method name.</param>
    /// <param name="registerMethodCalls">The register method calls for registrations in this assembly.</param>
    /// <param name="assemblyMethodCalls">The assembly method calls for registrations of referenced assemblies.</param>
    /// <returns>The dependency injection source code.</returns>
    internal static string MergeRegistrationSourceCode(string assemblyName, IEnumerable<string?> registerMethodCalls, IEnumerable<string> assemblyMethodCalls)
    {
        using StringWriter source = new();

        string sanitizedAssemblyName = Sanitize(assemblyName);
        string registeredServicesFieldName = $"registeredServices{sanitizedAssemblyName}";

        source.WriteLine("// <auto-generated />");
        source.WriteLine();
        source.WriteLine("using Microsoft.Extensions.DependencyInjection;");
        source.WriteLine();
        source.WriteLine("namespace Mdk.DISourceGenerator;");
        source.WriteLine();
        source.WriteLine($"public static partial class {RegistrationStaticClassName}");
        source.WriteLine("{");
        source.WriteLine($"    public static IServiceCollection RegisterServices{sanitizedAssemblyName}(this IServiceCollection services)");
        source.WriteLine("    {");
        source.WriteLine($"        if ({registeredServicesFieldName})");
        source.WriteLine("            return services;");
        source.WriteLine();

        foreach (string assemblyMethodCall in assemblyMethodCalls)
        {
            source.WriteLine($"        {assemblyMethodCall};");
        }
        if (assemblyMethodCalls.Any())
        {
            source.WriteLine();
        }

        foreach (string? methodCall in registerMethodCalls)
        {
            source.WriteLine(methodCall is not null ? $"        services.{methodCall};" : null);
        }
        if (registerMethodCalls.Any())
        {
            source.WriteLine();
        }

        source.WriteLine($"        {registeredServicesFieldName} = true;");
        source.WriteLine();
        source.WriteLine("        return services;");
        source.WriteLine("    }");
        source.WriteLine();
        source.WriteLine($"    private static bool {registeredServicesFieldName};");
        source.WriteLine("}");

        return source.ToString();
    }

    /// <summary>Sanitizes the name by removing dots, so name can be part of method names.</summary>
    private static string Sanitize(string name) => name.Replace(".", "");

    internal readonly static string RegistrationStaticClassName = "DIRegistrations";
}