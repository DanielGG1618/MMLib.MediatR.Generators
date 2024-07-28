﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class MethodDeclarationSyntaxExtensions
{
    public static IEnumerable<AttributeSyntax> Attributes(this MethodDeclarationSyntax methodDeclaration) =>
        methodDeclaration.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
}
