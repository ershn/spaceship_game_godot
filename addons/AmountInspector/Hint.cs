using System;

namespace AmountInspector;

public static class Hint
{
    public enum Type
    {
        Any,
        Mass,
        Count,
        ModeOf
    }

    public const string Prefix = "Amount&";

    public const string Any = $"{Prefix}Any";
    public const string Mass = $"{Prefix}Mass";
    public const string Count = $"{Prefix}Count";
    public const string ModeOf = $"{Prefix}ModeOf=";

    public static bool IsValid(string value) => value.StartsWith(Prefix);

    public static Type Parse(string hint, out string param)
    {
        var parts = hint[Prefix.Length..].Split('=');
        var type = Enum.Parse<Type>(parts[0]);
        param = parts.Length > 1 ? parts[1] : null;
        return type;
    }
}
