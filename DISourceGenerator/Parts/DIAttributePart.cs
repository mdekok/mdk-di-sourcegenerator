using Microsoft.CodeAnalysis;
using System.Linq;

namespace Mdk.DISourceGenerator.Parts;

/// <summary>Abstract DI attribute part.</summary>
internal abstract class DIAttributePart(AttributeData attribute) : IDIPart
{
    /// <summary>Gets the name of the generic parameter of the attribute, if the type is defined as a generic parameter. Like in [AddScoped<ServiceType>].</summary>
    protected abstract string AttributeParameterTypeName { get; }

    /// <summary>Gets the index of the constructor argument of the attribute, if the type is defined as constructor argument. Like in [AddScoped(Type serviceType)].</summary>
    protected abstract int AttributeParameterIndex { get; }

    /// <summary>Gets the type of the specified service or implementation type.</summary>
    private INamedTypeSymbol? AttributeParameterType
    {
        get
        {
            if (this._attributeParameterType is not null)
                return this._attributeParameterType;

            INamedTypeSymbol attributedClass = attribute.AttributeClass!;

            // Get type as generic type: [AddScoped<ServiceType, ImplementationType>]
            if (attributedClass.TypeParameters.FirstOrDefault(
                symbol => symbol.Name == this.AttributeParameterTypeName) is ITypeParameterSymbol typeParameterSymbol)
                return this._attributeParameterType = attributedClass.TypeArguments[attributedClass.TypeParameters.IndexOf(typeParameterSymbol)] as INamedTypeSymbol;

            // Get type as constructor parameter: [AddScoped(typeof(ServiceType), typeof(ImplementationType))]
            if (attribute.ConstructorArguments.Length > AttributeParameterIndex)
                return this._attributeParameterType = attribute.ConstructorArguments[AttributeParameterIndex].Value as INamedTypeSymbol;

            return null;
        }
    }
    private INamedTypeSymbol? _attributeParameterType;

    /// <summary>Gets a value indicating whether the parameter is defined on the DIAttribute.</summary>
    public bool IsDefined => this.AttributeParameterType is not null;

    /// <summary>Gets a value indicating whether the parameter is a unbound generic type.</summary>
    public bool IsUnboundGeneric => this.AttributeParameterType?.IsUnboundGenericType ?? false;

    /// <inheritdoc />
    public bool IsGeneric => this.AttributeParameterType?.IsGenericType ?? false;

    /// <inheritdoc />
    public string ToSource()
    {
        if (this._asString is not null)
            return this._asString;

        if (this.AttributeParameterType is not INamedTypeSymbol attributeParameterType)
            return this._asString = string.Empty;

        if (attributeParameterType is not null)
        {
            string? genericPart = null;
            if (attributeParameterType.IsUnboundGenericType)
            {
                genericPart = $"<{new string(',', attributeParameterType.TypeArguments.Length - 1)}>";
            }
            else if (attributeParameterType.IsGenericType)
            {
                genericPart = $"<{string.Join(", ", attributeParameterType.TypeArguments.Select(type => type.OriginalDefinition))}>";
            }

            return this._asString = $"global::{attributeParameterType.ContainingNamespace}.{attributeParameterType.Name}{genericPart}";
        }

        return this._asString = string.Empty;
    }
    private string? _asString;
}