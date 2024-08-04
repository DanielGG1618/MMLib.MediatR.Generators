using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    public static string Name(this TypeDeclarationSyntax type) =>
        type.Identifier.Text;

    public static IEnumerable<AttributeSyntax> Attributes(this TypeDeclarationSyntax type) =>
        type.AttributeLists.SelectMany(list => list.Attributes);

    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        ISet<string> names
    ) => type.Attributes().ContainsAttributeWithNameFrom(names);
}