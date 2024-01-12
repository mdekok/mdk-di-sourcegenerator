using Mdk.DISourceGenerator.Lib;
using Mdk.DISourceGenerator.Validation.Validators;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Mdk.DISourceGenerator.Validation;

/// <summary>DIRegistration validator.</summary>
public static class DIRegistrationValidator
{
    /// <summary>Validates a registration.</summary>
    /// <param name="registration">The registration.</param>
    /// <param name="sourceContext">Information on the source around the DIAttribute.</param>
    /// <returns>A bool.</returns>
    public static bool Validate(DIRegistration registration, SourceContext sourceContext)
    {
        return true;

        bool result = true;
        Location? location = null;

        foreach (DiagnosticDescriptor? diagnosticDescriptor in Validations(registration, sourceContext))
        {
            if (diagnosticDescriptor is null)
                continue;

            location ??= sourceContext.ClassDeclarationSyntax.GetLocation();
            sourceContext.Context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, location));

            if (diagnosticDescriptor.DefaultSeverity == DiagnosticSeverity.Error)
            {
                result = false;
            }
        }

        return result;
    }

    /// <summary>Results of various validations on a registration.</summary>
    /// <param name="registration">The registration.</param>
    /// <param name="sourceContext">The source context.</param>
    /// <returns>A list of DiagnosticDescriptor?.</returns>
    public static IEnumerable<DiagnosticDescriptor?> Validations(DIRegistration registration, SourceContext sourceContext)
    {
        yield return DI001ValidatorMissingImplementationType.Validate(registration, sourceContext);
        yield return DI002ValidatorServiceInterfaceNotImplemented.Validate(registration, sourceContext);
    }
}