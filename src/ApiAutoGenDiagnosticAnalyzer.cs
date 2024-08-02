using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoApiGen;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiAutoGenDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.LiteralExpressionRequired);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.MethodDeclaration);
    }

    //TODO make this useful
    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (methodDeclaration.Identifier.Text is not "ForbiddenMethod")
            return;

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.LiteralExpressionRequired,
            methodDeclaration.Identifier.GetLocation(),
            methodDeclaration.Identifier.Text
        );

        context.ReportDiagnostic(diagnostic);
    }
}