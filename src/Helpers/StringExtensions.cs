using System;

namespace AutoApiGen.Helpers;

public static class StringExtensions
{
    private const string _controller = "Controller";

    public static string CheckControllerName(this string controllerName)
        => controllerName.EndsWith(_controller, StringComparison.OrdinalIgnoreCase)
            ? controllerName : $"{controllerName}{_controller}";
}