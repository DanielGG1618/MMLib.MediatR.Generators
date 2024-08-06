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
        RenderWithTemplate(new
            {
                Controller = controller,
                controller.BaseRoute
            },
            templatesProvider.Get()
        );
    
    private static string RenderWithTemplate(object obj, Template template) =>
        template.Render(CreateContext(obj));
    
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