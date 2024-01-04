using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>DI registration dictionary.</summary>
internal class DIRegistrationDictionary(SourceProductionContext context)
    : Dictionary<string, DIRegistration>
{
    /// <summary>Adds the registration to dictionary checking on multiple service registrations.</summary>
    /// <param name="registration">The registration to add.</param>
    public void Add(DIRegistration registration)
    {
        string key = registration.ServiceType.ToSource();
        if (this.TryGetValue(key, out DIRegistration firstRegistration))
        {
            // Multiple registrations for the same service type.
            context.ReportDiagnostic(
                Diagnostic.Create(DiagnosticsDescriptors.ServiceTypeRegisteredMultipleTimes, firstRegistration.SourceLocation, firstRegistration.ServiceType));
            context.ReportDiagnostic(
                Diagnostic.Create(DiagnosticsDescriptors.ServiceTypeRegisteredMultipleTimes, registration.SourceLocation, registration.ServiceType));
        }

        this.Add(key, registration);
    }
}