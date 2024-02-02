using Mdk.DISourceGenerator.Analyzers;
using Mdk.DISourceGenerator.Analyzers.Lib;
using System.Collections.Generic;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DIRegistration validator.</summary>
public static class DIRegistrationValidator
{
    /// <summary>Validates a registration.</summary>
    /// <param name="registration">The registration.</param>
    /// <returns>A bool.</returns>
    public static bool Validate(DIRegistration registration)
    {
        foreach (ValidationResult? validationResult in Validations(registration))
        {
            if (validationResult is not null
                && !validationResult.Value.CanGenerateSource)
                return false;
        }

        return true;
    }

    /// <summary>Validation results of an attribute.</summary>
    /// <param name="registration">The DI registration (class, service and implementation type).</param>
    /// <returns>Validation results.</returns>
    public static IEnumerable<ValidationResult?> Validations(DIRegistration registration)
    {
        yield return DI0001ImplementationTypeMissingAnalyzer.Validate(registration);
        yield return DI0002ServiceInterfaceNotImplementedAnalyzer.Validate(registration);
        yield return DI0003ImplementationIsNotClassTypeAnalyzer.Validate(registration);
        yield return DI0004ClassTypeNotServiceClassTypeOrSubClassAnalyzer.Validate(registration);
    }
}