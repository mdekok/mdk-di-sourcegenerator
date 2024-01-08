using Mdk.DISourceGenerator.Parts;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DI registration of a single service registration.</summary>
public class DIRegistration(string method, IDIPart serviceType, IDIPart? implementationType = null, bool doNotGenerateAsGeneric = false)
{
    /// <summary>Gets the registration method: AddSingleton, AddScoped or AddTransient.</summary>
    public string Method { get; } = method;

    /// <summary>Gets the service type.</summary>
    public IDIPart ServiceType { get; } = serviceType;

    /// <summary>Gets the implementation type.</summary>
    public IDIPart? ImplementationType { get; } = implementationType;

    /// <summary>Gets a value indicating whether source must not be generated in generic style.</summary>
    public bool DoNotGenerateAsGeneric { get; } = doNotGenerateAsGeneric;
}

/// <summary>DIRegistration extension methods.</summary>
public static class DIRegistrationExtensions
{
    /// <summary>Converts registration to source.</summary>
    /// <returns>A registration method call.</returns>
    public static string ToSource(this DIRegistration registration)
    {
        string method = registration.Method;
        string serviceType = registration.ServiceType.ToSource();
        string? implementationType = registration.ImplementationType?.ToSource();
        bool doNotGenerateAsGeneric = registration.DoNotGenerateAsGeneric;

        if (implementationType is not null && implementationType != serviceType)
            // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
            // or [Add{Lifetime}<ServiceType, ImplementationType>]
            return doNotGenerateAsGeneric
                ? $"{method}(typeof({serviceType}), typeof({implementationType}))"
                : $"{method}<{serviceType}, {implementationType}>()";

        // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
        // or [Add{Lifetime}<ServiceType, ImplementationType>]
        return doNotGenerateAsGeneric
            ? $"{method}(typeof({serviceType}))"
            : $"{method}<{serviceType}>()";
    }
}