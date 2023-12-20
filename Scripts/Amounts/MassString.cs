using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class MassString
{
    struct MassUnit
    {
        public string Symbol;
        public ulong Value;
    }

    static readonly MassUnit[] s_massUnits =
    {
        new() { Symbol = "t", Value = 1_000_000_000 },
        new() { Symbol = "kg", Value = 1_000_000 },
        new() { Symbol = "g", Value = 1000 },
        new() { Symbol = "mg", Value = 1 },
    };

    static readonly Regex s_whitespaceRegex = new(@"\s");
    static readonly Regex s_massValueRegex;

    static MassString()
    {
        var massUnitsRegex = string.Join('|', s_massUnits.Select(massUnit => massUnit.Symbol));
        s_massValueRegex = new Regex($@"\A(?<digits>[0-9]+(\.[0-9]+)?)(?<unit>{massUnitsRegex})\z");
    }

    public static string Format(ulong mass)
    {
        ulong divider = 10;
        if (mass > 0)
        {
            while (mass % divider == 0)
                divider *= 10;
        }

        var massUnit = s_massUnits.First(massUnit => divider > massUnit.Value);
        return $"{mass / massUnit.Value} {massUnit.Symbol}";
    }

    public static bool TryParse(string str, out ulong mass)
    {
        mass = 0;

        str = s_whitespaceRegex.Replace(str, "");
        var match = s_massValueRegex.Match(str);
        if (!match.Success)
            return false;

        var digits = match.Groups["digits"].Value;
        if (!decimal.TryParse(digits, out var decimalValue))
            return false;

        var unit = match.Groups["unit"].Value;
        var multiplier = s_massUnits.First(massUnit => massUnit.Symbol == unit).Value;

        mass = (ulong)Math.Floor(decimalValue * multiplier);
        return true;
    }
}
