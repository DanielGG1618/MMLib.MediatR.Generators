using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class ClassDeclarationSyntaxExtensions
{
    public static string GetMethodName(this ClassDeclarationSyntax classDeclarationSyntax) =>
        classDeclarationSyntax.Parent switch
        {
            ClassDeclarationSyntax parentClassDeclaration =>
                parentClassDeclaration.Identifier.Text,

            NamespaceDeclarationSyntax namespaceDeclaration =>
                $"{namespaceDeclaration.Name}_{classDeclarationSyntax.Identifier}",

            _ => $"{classDeclarationSyntax.Identifier.Text}Method" 
        };
}