using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class RoslynExtensions
{
    public static bool HaveAnyOfAttributes(this TypeDeclarationSyntax typeDeclaration, ISet<string> attributesName) =>
        typeDeclaration.AttributeLists.Count > 0
        && typeDeclaration.AttributeLists
            .SelectMany(WithAttributesNames(attributesName))
            .Any();

    public static AttributeSyntax? GetAttribute(this TypeDeclarationSyntax typeDeclaration, string attributeName)
        => typeDeclaration.GetAttributesWithName(attributeName).FirstOrDefault();

    public static AttributeSyntax GetAttribute(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> attributesNames
    ) => typeDeclaration.GetAttributesWithNames(attributesNames).First();

    public static string GetTypeName(this TypeDeclarationSyntax typeDeclaration)
        => typeDeclaration.Identifier.Text;

    public static string? GetFirstArgumentWithoutName(this AttributeSyntax attribute)
    {
        var value = attribute
            .ArgumentList?
            .Arguments
            .FirstOrDefault(p => p.NameEquals is null)?
            .Expression as LiteralExpressionSyntax;

        return value?.Token.ValueText;
    }

    public static string? GetStringArgument(
        this AttributeSyntax attribute,
        string argumentName
    ) => attribute
        .GetArgument<LiteralExpressionSyntax>(argumentName)?
        .Token.ValueText;

    public static Dictionary<string, ITypeSymbol> GetProperties(this INamedTypeSymbol symbol) =>
        symbol
            .GetAllMembers()
            .Where(x => x.Kind is SymbolKind.Property)
            .OfType<IPropertySymbol>()
            .ToDictionary(
                property => property.Name,
                property => property.Type,
                StringComparer.OrdinalIgnoreCase
            );

    public static MethodDeclarationSyntax? GetMethodSymbol(this TypeDeclarationSyntax typeDeclaration, string name) =>
        typeDeclaration
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == name);

    public static T? GetArgument<T>(this AttributeSyntax attribute, string argumentName)
        where T : ExpressionSyntax =>
        attribute
            .ArgumentList?
            .Arguments
            .FirstOrDefault(argument =>
                argument.NameEquals?.Name.Identifier.ValueText == argumentName
            )?.Expression as T;

    public static INamedTypeSymbol? GetTypeArgument(
        this AttributeSyntax attribute,
        string argumentName,
        SemanticModel semanticModel
    )
    {
        var typeOfExpression = attribute.GetArgument<TypeOfExpressionSyntax>(argumentName)?.Type;

        if (typeOfExpression is null)
            return null;
        
        TypeInfo typeInfo = semanticModel.GetTypeInfo(typeOfExpression);
        return typeInfo.Type as INamedTypeSymbol;
    }
    
    private static IEnumerable<ITypeSymbol> WithBaseTypes(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }
    
    private static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type) => 
        type.WithBaseTypes().SelectMany(t => t.GetMembers());
    
    
    private static IEnumerable<AttributeSyntax> GetAttributesWithName(
        this TypeDeclarationSyntax typeDeclaration,
        string attributeName
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithAttributeName(attributeName));

    private static IEnumerable<AttributeSyntax> GetAttributesWithNames(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> attributesNames
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithAttributesNames(attributesNames));
    
    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithAttributeName(
        string attributeName
    ) => attributeList =>
        attributeList?.Attributes.Where(attribute =>
            (attribute.Name as IdentifierNameSyntax)?.Identifier.Text == attributeName
        )
        ?? [];

    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithAttributesNames(
        ISet<string> attributes
    ) => attributeList =>
        attributeList?.Attributes.Where(attribute =>
            attributes.Contains((attribute.Name as IdentifierNameSyntax)?.Identifier.Text ?? string.Empty)
        )
        ?? [];
}