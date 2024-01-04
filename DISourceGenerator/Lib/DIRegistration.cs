using Mdk.DISourceGenerator.Parts;
using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DI registration of a single service registration.</summary>
public class DIRegistration(string method, IDIPart serviceType, IDIPart? implementationType = null, bool doNotShowAsGeneric = false)
{
    /// <summary>Gets the registration method: AddSingleton, AddScoped or AddTransient.</summary>
    public string Method { get; } = method;

    /// <summary>Gets the service type.</summary>
    public IDIPart ServiceType { get; } = serviceType;

    /// <summary>Gets the implementation type.</summary>
    public IDIPart? ImplementationType { get; } = implementationType;

    /// <summary>Gets or sets the source location of the class the attribute is assigned to.</summary>
    public Location? SourceLocation { get; set; }

    /// <summary>Converts registration to source.</summary>
    /// <returns>A registration method call.</returns>
    public string ToSource()
    {
        if (_source is not null)
            return _source;

        if (this.ImplementationType is not null
            && this.ImplementationType.ToSource() != this.ServiceType.ToSource())
            // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
            // or [Add{Lifetime}<ServiceType, ImplementationType>]
            return _source = doNotShowAsGeneric
                ? $"{Method}(typeof({ServiceType.ToSource()}), typeof({ImplementationType.ToSource()}))"
                : $"{Method}<{ServiceType.ToSource()}, {ImplementationType.ToSource()}>()";

        // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
        // or [Add{Lifetime}<ServiceType, ImplementationType>]
        return _source = doNotShowAsGeneric
            ? $"{Method}(typeof({ServiceType.ToSource()}))"
            : $"{Method}<{ServiceType.ToSource()}>()";
    }
    private string? _source;
}
