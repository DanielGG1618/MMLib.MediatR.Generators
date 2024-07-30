using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class TypeDeclarationSyntaxExtensions
{
    //TODO think about relocating this maybe to some concrete type 
    public static string GetEndpointMethodName(this TypeDeclarationSyntax type) =>
        type.Parent switch
        {
            TypeDeclarationSyntax parentClass => parentClass.Name(),
            NamespaceDeclarationSyntax @namespace => $"{@namespace.Name.NameOrDefault()}_{type.Name()}",
            _ => $"{type.Name()}Method"
        };

    public static string GetControllerName(this TypeDeclarationSyntax type, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(type.SyntaxTree);

        var argumentExpression = type.AttributesWithNames(RenameThisClass.EndpointAttributeNames).First()
            .ArgumentList?.Arguments
            .First(arg =>
                arg.GetParameterName(
                    semanticModel
                )
                == "Route"
            ).Expression;

        return argumentExpression switch
        {
            LiteralExpressionSyntax literal => literal.Token.ValueText,
            null => throw new NullReferenceException(),
            _ => ReportDiagnosticAndReturnDefault(argumentExpression)
        };

        static string ReportDiagnosticAndReturnDefault(ExpressionSyntax expressionSyntax)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.LiteralExpressionRequired,
                expressionSyntax.GetLocation()
            );

            var context = new SourceProductionContext();
            context.ReportDiagnostic(diagnostic);

            return string.Empty;
        }
    }

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
    
    public static AttributeSyntax? AttributeWithName(
        this TypeDeclarationSyntax typeDeclaration,
        string name
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithName(name))
        .SingleOrDefault();

    public static IEnumerable<AttributeSyntax> AttributesWithNames(
        this TypeDeclarationSyntax typeDeclaration,
        ISet<string> names
    ) => typeDeclaration
        .AttributeLists
        .SelectMany(WithNames(names));
    
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