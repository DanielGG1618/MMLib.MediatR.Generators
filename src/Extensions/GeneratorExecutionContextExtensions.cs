﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class GeneratorExecutionContextExtensions
{
    private static readonly DiagnosticDescriptor MissingArgument = new(
        id: "MMLG001",
        title: "Missing attribute argument",
        messageFormat: "Argument '{0}' of '{1}Attribute' is required",
        category: "AutoApiGen",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static void ReportMissingArgument(
        this GeneratorExecutionContext context,
        AttributeSyntax attribute,
        string argumentName
    ) => context.ReportDiagnostic(
        Diagnostic.Create(
            MissingArgument,
            attribute.GetLocation(),
            argumentName,
            (attribute.Name as IdentifierNameSyntax)?.Identifier.Text
        )
    );
}