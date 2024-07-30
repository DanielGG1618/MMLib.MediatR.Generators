﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class CompilationExtensions
{
    public static ImmutableArray<AttributeData>? GetAttributesOf(
        this Compilation compilation,
        ClassDeclarationSyntax classDeclarationSyntax
    ) => compilation
        .GetSemanticModel(classDeclarationSyntax.SyntaxTree)
        .GetDeclaredSymbol(classDeclarationSyntax)?
        .GetAttributes();
}