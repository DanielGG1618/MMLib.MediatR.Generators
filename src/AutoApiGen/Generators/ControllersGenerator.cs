﻿using AutoApiGen.DataObjects;
using AutoApiGen.Extensions;
using AutoApiGen.TemplatesProcessing;
using AutoApiGen.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Generators;

[Generator]
internal class ControllersGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
                node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } @class
                && @class.HasAttributeWithNameFrom(EndpointAttributeNames),
            
            transform: static (syntaxContext, _) =>
                EndpointContractDeclarationSyntax.Wrap((TypeDeclarationSyntax)syntaxContext.Node)
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
        var (compilation, endpoints) = compilationDetails;
        
        var controllers = new Dictionary<string, ControllerData>();
        
        foreach (var endpoint in endpoints)
        { 
            var httpMethod = endpoint.GetHttpMethod();
            var methodName = endpoint.GetMethodName();
            
            var controllerName = endpoint.GetControllerName();
            var baseRoute = ""; //TODO this has to be implemented somehow
            
            var method = new MethodData(
                HttpMethod: httpMethod,
                Attributes: [],
                Name: methodName,
                Parameters: [],
                RequestType: "string",
                ResponseType: "string"
            );

            controllers[controllerName] = controllers.TryGetValue(controllerName, out var controller)
                ? controller with { Methods = [method, ..controller.Methods] }
                : new ControllerData(
                    baseRoute,
                    controllerName,
                    [method]
                );
        }
        
        foreach (var controller in controllers.Values)
        {
            context.AddSource(
                $"{controller.Name}.g.cs",
                SourceCodeGenerator.Generate(controller, templatesProviders)
            );
        }
    }
}