using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class AttributeSyntaxExtensions
{
    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        ISet<string> names
    ) => attributes.Any(attribute => names.Contains(attribute.Name.NameOrDefault()));


    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        ISet<string> names,
        out AttributeSyntax attribute
    ) => (attribute =
            attributes.FirstOrDefault(attribute =>
                names.Contains(attribute.Name.NameOrDefault())
            )!
        ) is not null;
}
