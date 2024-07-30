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
                && @class.HasAttributeWithAnyNameFrom(RenameThisClass.EndpointAttributeNames),
            transform: static (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node
        );

        var aaa = provider.Collect();
        var compilation = context.CompilationProvider.Combine(aaa);

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<ClassDeclarationSyntax>) compilationDetails
    )
    {
        //context.AddSource("SourceTypes.cs", EmbeddedResource.GetContent("Controllers.SourceTypes.cs"));

        var (compilation, classes) = compilationDetails;
        var templatesProviders = new EmbeddedResourceTemplatesProvider();
        var controllers = new Dictionary<string, ControllerModel>();
        
        foreach (var @class in classes)
        {
            var controllerName = @class.GetControllerName(compilation, context);

            if (controllers.TryGetValue(controllerName, out var controller))
                controller = controller with { Name = controllerName };
            else
                controller = new ControllerModel(controllerName, []);
            
            controllers.Add(controllerName, controller);
        }
        
        foreach (var controller in controllers.Values)
            context.AddSource($"{controller.Name}.g.cs", SourceCodeGenerator.Generate(controller, templatesProviders));
    }
}
