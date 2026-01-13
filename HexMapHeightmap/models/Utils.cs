namespace com.hexagonsimulations.HexMapHeightmap.Models;

internal class Utils
{
    /// <summary>
    /// Fade function for smooth interpolation (6t^5 - 15t^4 + 10t^3)
    /// </summary>
    internal static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    /// <summary>
    /// Linear interpolation between two values
    /// </summary>
    internal static double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    /// <summary>
    /// Gradient function that converts hash value to dot product with distance vector
    /// </summary>
    internal static double Grad(int hash, double x, double y)
    {
        // Take the lower 4 bits of hash and use it to select a gradient direction
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
