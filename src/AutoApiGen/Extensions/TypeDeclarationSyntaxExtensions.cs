using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    public static string Name(this TypeDeclarationSyntax typeDeclaration) =>
        typeDeclaration.Identifier.Text;

    public static MethodDeclarationSyntax? MethodWithName(this TypeDeclarationSyntax typeDeclaration, string name) =>
        typeDeclaration
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .SingleOrDefault(method => method.Name() == name);
    
    public static bool HasAttributeWithAnyNameFrom(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> names
    ) => typeDeclaration.AttributeLists
        .SelectMany(WithNames(names))
        .Any();
    
    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithName(
        string attributeName
    ) => attributeList =>
        attributeList.Attributes.Where(attribute =>
            attributeName == attribute.Name.NameOrDefault()
        );

    private static Func<AttributeListSyntax, IEnumerable<AttributeSyntax>> WithNames(
        ISet<string> names
    ) => attributeList =>
        attributeList.Attributes.Where(attribute =>
            names.Contains(attribute.Name.NameOrDefault())
        );
}