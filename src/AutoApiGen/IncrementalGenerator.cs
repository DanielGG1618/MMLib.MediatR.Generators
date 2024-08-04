using AutoApiGen.DataObjects;
using AutoApiGen.Extensions;
using AutoApiGen.TemplatesProcessing;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen;

[Generator]
internal class IncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();
        
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } @class
                && @class.HasAttributeWithAnyNameFrom(RenameThisClass.EndpointAttributeNames),
            
            transform: static (syntaxContext, _) =>
                EndpointContractDeclarationSyntax.Wrap((ClassDeclarationSyntax)syntaxContext.Node)
        );

        var compilationDetails = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilationDetails, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<EndpointContractDeclarationSyntax>) compilationDetails
    )
    {
        var templatesProviders = new EmbeddedResourceTemplatesProvider();
        var (compilation, handlers) = compilationDetails;
        
        context.AddSource("Start.g.cs", "namespace TempConsumer; public interface IStartMarker;");
        context.AddSource("ApiController.g.cs", EmbeddedResource.GetContent("Templates.ApiControllerBase.txt"));
        context.AddSource("EndpointAttributes.g.cs", EmbeddedResource.GetContent("Templates.EndpointAttributes.txt"));
        
        var controllers = new Dictionary<string, ControllerData>();
        
        foreach (var handler in handlers)
        { 
            var controllerName = handler.GetControllerName();
            var baseRoute = ""; //TODO this has to be implemented somehow
            var endpointMethod = new MethodData(
                HttpMethod: "Get",
                Attributes: [],
                Name: "Method",
                Parameters: [],
                RequestType: "string",
                ResponseType: "string"
            );

            controllers[controllerName] = controllers.TryGetValue(controllerName, out var controller)
                ? controller with
                {
                    Methods =
                    [
                        endpointMethod with { Name = endpointMethod.Name + controller.Methods.Count },
                        ..controller.Methods
                    ]
                }
                : new ControllerData(
                    baseRoute,
                    controllerName,
                    [endpointMethod]
                );
        }
        
        foreach (var controller in controllers.Values)
        {
            context.AddSource(
                $"{controller.Name}.g.cs",
                SourceCodeGenerator.Generate(controller, templatesProviders)
            );
        }
        
        context.AddSource("End.g.cs", "namespace TempConsumer; public interface IEndMarker;");
    }
}
