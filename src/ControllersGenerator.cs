using AutoApiGen.Extensions;
using AutoApiGen.Internal;
using AutoApiGen.Internal.Models;
using AutoApiGen.Internal.Static;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen;

[Generator]
public class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } @class
                && @class.HaveAttributeWithAnyNameFrom(HttpMethods.AttributeNames),
            transform: static (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node
        );

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<ClassDeclarationSyntax>) compilationDetails
    )
    {
        context.AddSource("SourceTypes.cs", EmbeddedResource.GetContent("Controllers.SourceTypes.cs"));
        
        var (compilation, classesDeclarationSyntaxes) = compilationDetails;

        var templates = new Templates();
        var controller = default(ControllerModel)!; //should be generated from templates
        context.AddSource($"{controller.Name}", SourceCodeGenerator.Generate(controller, templates));
    }
}
