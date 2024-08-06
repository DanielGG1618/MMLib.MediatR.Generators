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
    
    
    public static bool HasAttributeWithNameFrom(
        this TypeDeclarationSyntax type,
        ISet<string> names,
        out AttributeSyntax attribute
    ) => type.Attributes().ContainsAttributeWithNameFrom(names, out attribute);

    public static IEnumerable<string> GetGenericTypeParametersOfInterface(
        this TypeDeclarationSyntax type,
        string interfaceName
    ) => type.BaseList?.Types
             .Where(baseType =>
                 baseType.Type is GenericNameSyntax genericName 
                 && genericName.Identifier.Text == interfaceName
             )
             .SelectMany(baseType => ((GenericNameSyntax)baseType.Type).TypeArgumentList.Arguments)
             .Select(typeArgument => typeArgument.ToString())
         ?? [];
}