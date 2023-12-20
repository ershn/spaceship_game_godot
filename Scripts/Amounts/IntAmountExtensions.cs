public static class IntAmountExtensions
{
    public static ulong MilliGram(this int value) => (ulong)value;

    public static ulong MilliGrams(this int value) => (ulong)value;

    public static ulong Gram(this int value) => (ulong)value * 1000;

    public static ulong Grams(this int value) => (ulong)value * 1000;

    public static ulong KiloGram(this int value) => (ulong)value * 1_000_000;

    public static ulong KiloGrams(this int value) => (ulong)value * 1_000_000;

    public static ulong Ton(this int value) => (ulong)value * 1_000_000_000;

    public static ulong Tons(this int value) => (ulong)value * 1_000_000_000;

    public static ulong Calorie(this int value) => (ulong)value;

    public static ulong Calories(this int value) => (ulong)value;

    public static ulong KiloCalorie(this int value) => (ulong)value * 1000;

    public static ulong KiloCalories(this int value) => (ulong)value * 1000;
}
