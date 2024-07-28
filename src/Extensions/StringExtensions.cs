namespace AutoApiGen.Extensions;

public static class StringExtensions
{
    private const string Controller = "Controller";

    public static string CheckControllerName(this string controllerName)
        => controllerName.EndsWith(Controller, StringComparison.OrdinalIgnoreCase)
            ? controllerName : $"{controllerName}{Controller}";
}