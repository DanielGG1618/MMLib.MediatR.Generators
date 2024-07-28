namespace AutoApiGen.Helpers;

internal static class Helper
{
    public static string GetAttributeName<TAttribute>() where TAttribute : Attribute =>
        typeof(TAttribute).Name.Replace("Attribute", string.Empty);
}