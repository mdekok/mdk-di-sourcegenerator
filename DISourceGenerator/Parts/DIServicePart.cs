using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Parts;

/// <summary>DIAttributePart of the service type.</summary>
internal class DIServicePart(AttributeData attribute)
    : DIAttributePart(attribute)
{
    /// <inheritdoc />
    protected override string AttributeParameterTypeName => "ServiceType";

    /// <inheritdoc />
    protected override int AttributeParameterIndex => 0;
}