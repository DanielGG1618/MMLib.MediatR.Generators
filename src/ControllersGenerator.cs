﻿using System.IO;
using AutoApiGen.Internal;
using AutoApiGen.Internal.Models;
using AutoApiGen.Internal.Static;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AutoApiGen;

[Generator]
public class ControllersGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) =>
        context.RegisterForSyntaxNotifications(() => new ControllerReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("SourceTypes.cs",
            SourceText.From(EmbeddedResource.GetContent("Controllers.SourceTypes.cs"), Encoding.UTF8));

        if (context.SyntaxReceiver is not ControllerReceiver actorSyntaxReceiver) 
            return;
        
        var builder = ControllerModel.Builder(context);
        foreach (var candidate in actorSyntaxReceiver.Candidates) 
            builder.AddCandidate(candidate);

        var templates = LoadTemplates(context);
        foreach (var controller in builder.Build(templates)) 
            context.AddSource($"{controller.Name}", SourceCodeGenerator.Generate(controller, templates));
    }

    private static Templates LoadTemplates(GeneratorExecutionContext context)
    {
        Templates templates = new();

        foreach (var file in context.AdditionalFiles)
        {
            if (!Path.GetExtension(file.Path).Equals(".txt", StringComparison.OrdinalIgnoreCase)) 
                continue;
            
            var options = context.AnalyzerConfigOptions.GetOptions(file);
            if (!options.TryGetValue("build_metadata.additionalfiles.MMLib_TemplateType", out var type) 
                || !Enum.TryParse(type, ignoreCase: true, out TemplateType templateType)) 
                continue;
            
            var controllerName = TryGetValue(options, "ControllerName");
            var template = file.GetText(context.CancellationToken)!.ToString(); //TODO I added ! here !!!

            if (templateType != TemplateType.MethodBody)
            {
                templates.AddTemplate(templateType, controllerName, template);
                continue;
            }

            var methodType = TryGetValue(options, "MethodType");
            var methodName = TryGetValue(options, "MethodName");
            templates.AddMethodBodyTemplate(controllerName, methodType, methodName, template);
        }

        return templates;
    }

    private static string TryGetValue(AnalyzerConfigOptions options, string type)
        => options.TryGetValue($"build_metadata.additionalfiles.MMLib_{type}", out var value) 
            ? value 
            : string.Empty;
}