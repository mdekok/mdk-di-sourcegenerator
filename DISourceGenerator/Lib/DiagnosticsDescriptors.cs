using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib;

#pragma warning disable RS2008 // Enable analyzer release tracking

/// <summary>The diagnostics descriptors.</summary>
internal static class DiagnosticsDescriptors
{
    public static readonly DiagnosticDescriptor MissingImplementationType
        = new("DI001",
            "Implementation type missing",
            "Add the implementation type to the DIAttribute",
            "Generator",
            DiagnosticSeverity.Error,
            true);
}

#pragma warning restore RS2008 // Enable analyzer release tracking