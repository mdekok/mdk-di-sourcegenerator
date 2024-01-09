using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DIRegistration validator.</summary>
public static class DIRegistrationValidator
{
    /// <summary>Validates a registration.</summary>
    /// <param name="registration">The registration.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="syntax">The class declaration syntax.</param>
    /// <returns>A bool.</returns>
    public static bool Validate(
        DIRegistration registration,
        SourceProductionContext context,
        ClassDeclarationSyntax syntax)
    {
        bool result = true;

        // [Add{Lifetime}<ServiceType<int>>] is not allowed.
        // should be [Add{Lifetime}<ServiceType<int>, ImplementationType>].
        if (registration.ServiceType.TypeKind == TypeKind.Interface
            && registration.ServiceType.IsGeneric
            && !registration.ServiceType.IsUnboundGeneric
            && registration.ImplementationType is null)
        {
            result = false;
            ReportDiagnostic(DiagnosticsDescriptors.MissingImplementationType);
        }

        return result;

        // Local function to report diagnostics.
        void ReportDiagnostic(DiagnosticDescriptor descriptor, params object[] messageArgs)
            => context.ReportDiagnostic(
                Diagnostic.Create(descriptor, syntax.GetLocation(), messageArgs));
    }
}