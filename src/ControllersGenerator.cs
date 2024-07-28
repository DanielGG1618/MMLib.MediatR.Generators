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
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
            transform: static (syntaxContext, _) => GetSemanticTargetForGeneration(syntaxContext)
        ).Where(static syntax => syntax is not null)!;

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            return classDeclarationSyntax.AttributeLists
                .SelectMany(attributeListSyntax =>
                    attributeListSyntax.Attributes.Select(attributeSyntax =>
                        context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol
                    )
                ).Select(syntax => syntax?.ContainingType)
                .OfType<INamedTypeSymbol>()
                .Any() ? classDeclarationSyntax : null;
        return null;
    }

    private static void Execute(
        SourceProductionContext context,
        (Compilation, ImmutableArray<ClassDeclarationSyntax>) compilationDetails
    )
    {
        var (compilation, classesDeclarationSyntaxes) = compilationDetails;

        /*var codeBuilder = new CodeBuilder();
        codeBuilder.AppendUsing("Microsoft.AspNetCore.Mvc");
        codeBuilder.AppendNamespace(nameof(CodeGenerator));*/

        foreach (var classDeclarationSyntax in classesDeclarationSyntaxes)
        {
            var attributeDatas = compilation
                .GetAttributesOf(classDeclarationSyntax)?
                .Where(attribute => attribute.AttributeClass?.BaseType?.Name == "EndpointAttribute");

            if (attributeDatas is null)
                continue;

            foreach (var attributeData in attributeDatas)
            {
                var controllerName = attributeData.GetControllerName();
                //var httpMethodName = attributeData.GetHttpMethodName();
                var route = attributeData.GetRoute();
                var methodName = classDeclarationSyntax.GetMethodName();

                /*codeBuilder.AppendLine($$"""
                                         //[ApiController]
                                         public partial class {{controllerName}} : ControllerBase
                                         {
                                             [Http{{httpMethodName}}("{{route}}")]
                                             public IActionResult {{methodName}}()
                                             {
                                                return Ok();
                                             }
                                         }

                                         """);*/
            }
        }

        context.AddSource("Controllers.g.cs", "codeBuilder".ToString());
    }

    public void Execute(SourceProductionContext context)
    {
        context.AddSource("SourceTypes.cs", EmbeddedResource.GetContent("Controllers.SourceTypes.cs"));

        var templates = LoadTemplates(context);
        var controller = new ControllerModel(); //should be generated from templates
        context.AddSource($"{controller.Name}", SourceCodeGenerator.Generate(controller, templates));
    }

    private static Templates LoadTemplates(SourceProductionContext context)
    {
        throw new NotImplementedException();
    }
}
