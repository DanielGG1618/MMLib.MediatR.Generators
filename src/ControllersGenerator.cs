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
                && @class.HasAttributeWithAnyNameFrom(HttpMethods.AttributeNames),
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
        //context.AddSource("SourceTypes.cs", EmbeddedResource.GetContent("Controllers.SourceTypes.cs"));

        var (compilation, classDeclarationSyntaxes) = compilationDetails;
        var templatesProviders = new EmbededResourceTemplatesProvider();
        var controllers = new Dictionary<string, ControllerModel>();
        
        foreach (var @class in classDeclarationSyntaxes)
        {
        }
        
        foreach (var controller in controllers.Values)
            context.AddSource($"{controller.Name}", SourceCodeGenerator.Generate(controller, templatesProviders));
    }
}
