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

    public static string GetControllerName(
        this TypeDeclarationSyntax type,
        Compilation compilation,
        SourceProductionContext context
    )
    {
        var semanticModel = compilation.GetSemanticModel(type.SyntaxTree);

        var attributes = type.AttributesWithNames(RenameThisClass.EndpointAttributeNames);
        attributes = attributes as AttributeSyntax[] ?? attributes.ToArray();

        if (!attributes.Any())
            return Report(type, context, "attributes are empty");

        var argumentList = attributes.First().ArgumentList;
        if (argumentList is null or { Arguments: { Count: 0 } })
            return Report(type, context, "argument list is empty");

        var argumentExpression = argumentList.Arguments.FirstOrDefault(arg =>
            arg.GetParameterName(semanticModel) is "Route"
        )?.Expression;

        if (argumentExpression is null)
            return Report(type, context, "Route argument is null");

        return argumentExpression switch
        {
            LiteralExpressionSyntax literal => literal.Token.ValueText,
            null => Report(type, context), //throw new NullReferenceException(),
            _ => ReportDiagnosticAndReturnDefault(argumentExpression, context)
        };

        static string Report(TypeDeclarationSyntax t, SourceProductionContext context, string message = "Random report")
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.ForDebug(message, DiagnosticSeverity.Error),
                t.GetLocation()
            );

            context.ReportDiagnostic(diagnostic);

            return string.Empty;
        }

        static string ReportDiagnosticAndReturnDefault(
            ExpressionSyntax expressionSyntax,
            SourceProductionContext context
        )
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.LiteralExpressionRequired,
                expressionSyntax.GetLocation()
            );

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
    ) => typeDeclaration.AttributesWithNames(names).Any();
    
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