﻿using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib;

#pragma warning disable RS2008 // Enable analyzer release tracking

/// <summary>The diagnostics descriptors.</summary>
internal static class DiagnosticsDescriptors
{
    // Usage:
    // context is SourceProductionContext, available in Execute method in the generator.
    // context.ReportDiagnostic(
    //   Diagnostic.Create(DiagnosticsDescriptors.ServiceTypeRegisteredMultipleTimes, firstRegistration.SourceLocation, firstRegistration.ServiceType));

    //public static readonly DiagnosticDescriptor ServiceTypeRegisteredMultipleTimes

    //    = new("DI001",
    //        "Same servicetype registered multiple times as DIAttribute",
    //        "Servicetype {0} is registered multiple times",
    //        "Generator",
    //        DiagnosticSeverity.Warning,
    //        true);
}

#pragma warning restore RS2008 // Enable analyzer release tracking