using AutoApiGen.Internal.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoApiGen.Internal.Static;

internal static class SourceCodeGenerator
{
    public static string Generate(ControllerModel controller, Templates templates)
    {
        var output = RenderBody(new
            {
                Usings = templates.Get(TemplateType.ControllerUsings, controller.Name),
                Attributes = RenderControllerAttributes(controller, templates),
                Body = RenderControllerBody(controller, templates),
                Controller = controller
            },
            templates.Get(TemplateType.Controller, controller.Name)
        );

        output = Format(output);

        return output;
    }
    
    public static string RenderBody(object body, string? templateSource)
    {
        var template = Template.Parse(templateSource);
        var context = CreateContext(body);

        return template.Render(context);
    }

    private static string Format(string output) =>
        ((CSharpSyntaxNode)
            CSharpSyntaxTree
                .ParseText(output)
                .GetRoot()
        ).NormalizeWhitespace(elasticTrivia: true)
        .ToFullString();

    private static string RenderControllerAttributes(ControllerModel controller, Templates templates) =>
        RenderBody(controller, templates.Get(TemplateType.ControllerAttributes, controller.Name));

    private static string RenderControllerBody(ControllerModel controller, Templates templates) =>
        RenderBody(new
            {
                Controller = controller,
                controller.Methods,
                templates
            },
            templates.Get(TemplateType.ControllerBody, controller.Name)
        );

    private static TemplateContext CreateContext(object body)
    {
        var context = new TemplateContext();

        var scriptObject = new ScriptObject();
        scriptObject.Import(body);
        context.PushGlobal(scriptObject);

        var functions = new ScribanFunctions();
        context.PushGlobal(functions);

        return context;
    }
}