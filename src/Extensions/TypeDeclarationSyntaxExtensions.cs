using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    //TODO think about relocating this maybe to some concrete type 
    public static string GetMethodName(this TypeDeclarationSyntax type) =>
        type.Parent switch
        {
            TypeDeclarationSyntax parentClass => parentClass.Name(),
            NamespaceDeclarationSyntax @namespace => $"{@namespace.Name}_{type.Name()}",
            _ => $"{type.Name()}Method"
        };
    
    public static string Name(this TypeDeclarationSyntax typeDeclaration)
        => typeDeclaration.Identifier.Text;
    
    public static MethodDeclarationSyntax? GetMethodDeclarationSyntaxWithName(
        this TypeDeclarationSyntax typeDeclaration,
        string name
    ) => typeDeclaration
        .DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .FirstOrDefault(m => m.Identifier.ValueText == name);
    
    public static bool HaveAttributeWithAnyNameFrom(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> names
    ) => typeDeclaration.AttributeLists
        .SelectMany(WithAttributeNames(names))
        .Any();
    
    public static AttributeSyntax? GetFirstAttributeWithName(this TypeDeclarationSyntax typeDeclaration, string name) =>
        typeDeclaration.GetAttributesWithName(name).FirstOrDefault();

    private static IEnumerable<AttributeSyntax> GetAttributesWithName(
        this TypeDeclarationSyntax typeDeclaration,
        string name
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithAttributeName(name));

    private static IEnumerable<AttributeSyntax> GetAttributesWithNames(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> names
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithAttributeNames(names));
    
    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithAttributeName(
        string attributeName
    ) => attributeList =>
        attributeList.Attributes.Where(attribute =>
            attributeName == attribute.Name.GetIdentifierNameOrDefault()
        );

    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithAttributeNames(
        ISet<string> names
    ) => attributeList =>
        attributeList.Attributes.Where(attribute =>
            names.Contains(attribute.Name.GetIdentifierNameOrDefault())
        );

}