using System.Diagnostics;
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
        if (!Debugger.IsAttached)
        {
            //Debugger.Launch();
        }
        
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } @class
                && @class.HasAttributeWithAnyNameFrom(RenameThisClass.EndpointAttributeNames),
            
            transform: static (syntaxContext, _) =>
                /*EndpointHandlerDeclarationSyntax.Wrap*/((ClassDeclarationSyntax)syntaxContext.Node)
        );

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<ClassDeclarationSyntax>) compilationDetails
    )
    {
        context.AddSource("Start.g.cs", "namespace TempConsumer; public interface IStartMarker;");
        //context.AddSource("SourceTypes.cs", EmbeddedResource.GetContent("Controllers.SourceTypes.cs"));

        var (compilation, classes) = compilationDetails;
        var templatesProviders = new EmbeddedResourceTemplatesProvider();
        var controllers = new Dictionary<string, ControllerModel>();
        
        foreach (var handler in classes.Select(EndpointHandlerDeclarationSyntax.Wrap))
        {
            var controllerName = handler.GetControllerName(compilation);
            var baseRoute = ""; //TODO this has to be implemented somehow
            var endpointMethod = new MethodModel(
                Name: "Method",
                HttpMethod: "Get",
                RequestType: "int",
                ResponseType: "int",
                Attributes: "",
                Parameters: [],
                RequestProperties: []
            );

            controllers[controllerName] = controllers.TryGetValue(controllerName, out var controller)
                ? controller with { Methods = [endpointMethod, ..controller.Methods] }
                : new ControllerModel(
                    controllerName,
                    baseRoute,
                    [/*endpointMethod*/]
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
