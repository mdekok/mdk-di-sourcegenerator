using Mdk.DISourceGenerator.Lib.Parts;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DI registration of a single service registration.</summary>
public sealed class DIRegistration(
    string method,
    IDIPart classType,
    IDIPart serviceType,
    IDIPart? implementationType = null,
    string? key = null,
    bool doNotGenerateAsGeneric = false)
{
    /// <summary>Gets the registration method: AddSingleton, AddScoped or AddTransient.</summary>
    public string Method { get; } = method;

    /// <summary>Gets the class type the attribute is assigned to.</summary>
    public IDIPart ClassType { get; } = classType;

    /// <summary>Gets the service type.</summary>
    public IDIPart ServiceType { get; } = serviceType;

    /// <summary>Gets the implementation type.</summary>
    public IDIPart? ImplementationType { get; } = implementationType;

    public string? Key { get; } = key;

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
        string? keyPartNonGeneric = registration.Key is null ? null : $", \"{registration.Key}\"";
        string? keyPartGeneric = registration.Key is null ? null : $"\"{registration.Key}\"";
        bool doNotGenerateAsGeneric = registration.DoNotGenerateAsGeneric;

        if (implementationType is not null && implementationType != serviceType)
            // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType), "key")]
            // or [Add{Lifetime}<ServiceType, ImplementationType>("key")]
            return doNotGenerateAsGeneric
                ? $"{method}(typeof({serviceType}), typeof({implementationType}){keyPartNonGeneric})"
                : $"{method}<{serviceType}, {implementationType}>({keyPartGeneric})";

        // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType), "key")]
        // or [Add{Lifetime}<ServiceType, ImplementationType>("key")]
        return doNotGenerateAsGeneric
            ? $"{method}(typeof({serviceType}){keyPartNonGeneric})"
            : $"{method}<{serviceType}>({keyPartGeneric})";
    }
}