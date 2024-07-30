using AutoApiGen.Internal.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoApiGen.Internal.Static;

internal static class SourceCodeGenerator
{
    public static string Generate(ControllerModel controller, ITemplatesProvider templatesProvider) =>
        Format(
            RenderWithTemplate(new
                {
                    Attributes = RenderControllerAttributes(controller, templatesProvider),
                    Body = RenderControllerBody(controller, templatesProvider),
                    Controller = controller
                },
                templatesProvider.Get(TemplateType.Controller)
            )
        );

    public static string RenderWithTemplate(object obj, Template template) =>
        template.Render(CreateContext(obj));

    private static string Format(string output) =>
        ((CSharpSyntaxNode)
            CSharpSyntaxTree
                .ParseText(output)
                .GetRoot()
        ).NormalizeWhitespace(elasticTrivia: true)
        .ToFullString();

    private static string RenderControllerAttributes(
        ControllerModel controller,
        ITemplatesProvider templatesProvider
    ) => RenderWithTemplate(controller, templatesProvider.Get(TemplateType.ControllerAttributes));

    private static string RenderControllerBody(ControllerModel controller, ITemplatesProvider templatesProvider) =>
        RenderWithTemplate(new
            {
                Controller = controller,
                controller.Methods,
                templates = templatesProvider
            },
            templatesProvider.Get(TemplateType.ControllerBody)
        );

    private static TemplateContext CreateContext(object body)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(body);
     
        var functions = new ScribanFunctions();

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        context.PushGlobal(functions);

        return context;
    }
}