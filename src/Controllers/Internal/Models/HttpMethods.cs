﻿using System.Collections.Generic;

namespace AutoApiGen.Controllers.Internal.Models;

internal static class HttpMethods
{
    public const string Get = "Get";
    public const string Post = "Post";
    public const string Put = "Put";
    public const string Delete = "Delete";
    public const string Patch = "Patch";

    public static ISet<string> Attributes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethod(Get),
        HttpMethod(Post),
        HttpMethod(Put),
        HttpMethod(Delete),
        HttpMethod(Patch)
    };

    private static string HttpMethod(string type) => $"Http{type}";
}