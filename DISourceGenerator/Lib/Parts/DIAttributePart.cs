using Microsoft.CodeAnalysis;
using System.Linq;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>Abstract DI attribute part.</summary>
internal abstract class DIAttributePart(AttributeData attribute) : DIPart
{
    /// <summary>Gets the name of the generic parameter of the attribute, if the type is defined as a generic parameter. Like in [AddScoped<ServiceType>].</summary>
    protected abstract string AttributeParameterTypeName { get; }

    /// <summary>Gets the index of the constructor argument of the attribute, if the type is defined as constructor argument. Like in [AddScoped(Type serviceType)].</summary>
    protected abstract int AttributeParameterIndex { get; }

    /// <inheritdoc />
    public override INamedTypeSymbol? NamedTypeSymbol
    {
        get
        {
            if (this._namedTypeSymbol is not null)
                return this._namedTypeSymbol;

            INamedTypeSymbol attributedClass = attribute.AttributeClass!;

            // Get type as generic type: [AddScoped<ServiceType, ImplementationType>]
            if (attributedClass.TypeParameters.FirstOrDefault(
                symbol => symbol.Name == this.AttributeParameterTypeName) is ITypeParameterSymbol typeParameterSymbol)
                return this._namedTypeSymbol = attributedClass.TypeArguments[attributedClass.TypeParameters.IndexOf(typeParameterSymbol)] as INamedTypeSymbol;

            // Get type as constructor parameter: [AddScoped(typeof(ServiceType), typeof(ImplementationType))]
            if (attribute.ConstructorArguments.Length > AttributeParameterIndex)
                return this._namedTypeSymbol = attribute.ConstructorArguments[AttributeParameterIndex].Value as INamedTypeSymbol;

            return null;
        }
    }
    private INamedTypeSymbol? _namedTypeSymbol;

    /// <summary>Gets a value indicating whether the parameter is defined on the DIAttribute.</summary>
    public bool IsDefined => this.NamedTypeSymbol is not null;

    /// <inheritdoc />
    public override string ToSource()
    {
        if (this._asString is not null)
            return this._asString;

        if (this.NamedTypeSymbol is not INamedTypeSymbol attributeParameterType)
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