using Mdk.DISourceGenerator.Lib;
using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Validation.Validators;

/// <summary>DI001 validator</summary>
internal static class DI001ValidatorMissingImplementationType
{
    /// <summary>Validates on missing implementation type.</summary>
    public static DiagnosticDescriptor? Validate(DIRegistration registration, SourceContext sourceContext)
    {
        // [Add{Lifetime}<ServiceType<int>>] is not allowed.
        // should be [Add{Lifetime}<ServiceType<int>, ImplementationType>].
        if (registration.ServiceType.TypeKind == TypeKind.Interface
            && registration.ServiceType.IsGeneric
            && !registration.ServiceType.IsUnboundGeneric
            && registration.ImplementationType is null)
            return MissingImplementationType;

        return null;
    }

    public static readonly DiagnosticDescriptor MissingImplementationType
        = new("DI001",
            "Implementation type missing",
            "Add the implementation type to the attribute, like [AddScoped<Interface, Implementation>]",
            "Generator",
            DiagnosticSeverity.Error,
            true);
}