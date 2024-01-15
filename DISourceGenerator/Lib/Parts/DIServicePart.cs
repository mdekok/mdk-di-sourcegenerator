using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>DIAttributePart of the service type.</summary>
public class DIServicePart(AttributeData attribute)
    : DIAttributePart(attribute)
{
    /// <inheritdoc />
    protected override string AttributeParameterTypeName => "ServiceType";

    /// <inheritdoc />
    protected override int AttributeParameterIndex => 0;
}