public static class MathUint
{
    public static uint ClampAdd(uint a, int b, uint max) =>
        b >= 0 ? ClampAdd(a, (uint)b, max) : ClampSub(a, (uint)-b);

    public static ulong ClampAdd(ulong a, long b, ulong max) =>
        b >= 0 ? ClampAdd(a, (ulong)b, max) : ClampSub(a, (ulong)-b);

    public static uint ClampAdd(uint a, uint b, uint max) => a + b > max ? max : a + b;

    public static ulong ClampAdd(ulong a, ulong b, ulong max) => a + b > max ? max : a + b;

    public static uint ClampSub(uint a, uint b) => a > b ? a - b : 0;

    public static ulong ClampSub(ulong a, ulong b) => a > b ? a - b : 0;
}
