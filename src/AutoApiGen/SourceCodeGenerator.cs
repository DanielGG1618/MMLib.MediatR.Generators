﻿using AutoApiGen.DataObjects;
using AutoApiGen.TemplatesProcessing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;
using Scriban.Runtime;

namespace AutoApiGen;

internal static class SourceCodeGenerator
{
    public static string Generate(ControllerData controller, ITemplatesProvider templatesProvider) =>
        Format(
            RenderWithTemplate(new
                {
                    Controller = controller,
                    controller.BaseRoute
                },
                templatesProvider.Get()
            )
        );
    
    private static string RenderWithTemplate(object obj, Template template) =>
        template.Render(CreateContext(obj));

    private static string Format(string text) =>
        ((CSharpSyntaxNode)
            CSharpSyntaxTree
                .ParseText(text)
                .GetRoot()
        ).NormalizeWhitespace(elasticTrivia: true)
        .ToFullString();
    
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