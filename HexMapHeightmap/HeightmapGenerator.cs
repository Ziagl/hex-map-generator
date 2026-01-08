using HexMapHeightmap.Models;

namespace HexMapHeightmap;

/// <summary>
/// Generates heightmap images using various noise algorithms
/// </summary>
public class HeightmapGenerator
{
    private readonly Random _random;
    private readonly int[] _permutation;
    private const int PermutationSize = 256;

    /// <summary>
    /// Creates a new HeightmapGenerator with an optional seed for reproducible results
    /// </summary>
    /// <param name="seed">Optional seed for random number generation. If null, a random seed is used.</param>
    public HeightmapGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        _permutation = InitializePermutationTable();
    }

    /// <summary>
    /// Initializes the permutation table for Perlin noise generation
    /// </summary>
    private int[] InitializePermutationTable()
    {
        var p = new int[PermutationSize];
        for (int i = 0; i < PermutationSize; i++)
        {
            p[i] = i;
        }

        // Fisher-Yates shuffle
        for (int i = PermutationSize - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }

        // Duplicate the permutation table to avoid overflow
        var permutation = new int[PermutationSize * 2];
        for (int i = 0; i < PermutationSize * 2; i++)
        {
            permutation[i] = p[i % PermutationSize];
        }

        return permutation;
    }

    /// <summary>
    /// Generates a white noise heightmap with the specified dimensions
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <returns>A 2D byte array containing grayscale heightmap values (0-255)</returns>
    public byte[,] GenerateWhiteNoise(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive values");
        }

        var heightmap = new byte[width, height];

        // Generate white noise - random grayscale value for each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heightmap[x, y] = (byte)_random.Next(0, 256);
            }
        }

        return heightmap;
    }

    /// <summary>
    /// Generates a Perlin noise heightmap with the specified dimensions and parameters
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="scale">Scale factor for the noise (higher values = more zoomed in). Default is 0.05.</param>
    /// <param name="octaves">Number of noise layers to combine for fractal detail. Default is 4.</param>
    /// <param name="persistence">Amplitude decay factor per octave (0-1). Default is 0.5.</param>
    /// <param name="lacunarity">Frequency increase factor per octave. Default is 2.0.</param>
    /// <returns>A 2D byte array containing grayscale heightmap values (0-255)</returns>
    public byte[,] GeneratePerlinNoise(int width, int height, double scale = 0.05, int octaves = 4, double persistence = 0.5, double lacunarity = 2.0)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive values");
        }

        if (scale <= 0)
        {
            throw new ArgumentException("Scale must be positive");
        }

        if (octaves < 1)
        {
            throw new ArgumentException("Octaves must be at least 1");
        }

        if (persistence < 0 || persistence > 1)
        {
            throw new ArgumentException("Persistence must be between 0 and 1");
        }

        if (lacunarity < 1)
        {
            throw new ArgumentException("Lacunarity must be at least 1");
        }

        var heightmap = new byte[width, height];
        double minValue = double.MaxValue;
        double maxValue = double.MinValue;
        var noiseValues = new double[width, height];

        // Generate Perlin noise values
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double amplitude = 1.0;
                double frequency = 1.0;
                double noiseValue = 0.0;

                // Combine multiple octaves for fractal noise
                for (int octave = 0; octave < octaves; octave++)
                {
                    double sampleX = x * scale * frequency;
                    double sampleY = y * scale * frequency;

                    double perlinValue = PerlinNoise2D(sampleX, sampleY);
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseValues[x, y] = noiseValue;
                minValue = Math.Min(minValue, noiseValue);
                maxValue = Math.Max(maxValue, noiseValue);
            }
        }

        // Normalize to 0-255 range
        double range = maxValue - minValue;
        if (range > 0)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double normalized = (noiseValues[x, y] - minValue) / range;
                    heightmap[x, y] = (byte)(normalized * 255);
                }
            }
        }
        else
        {
            // If all values are the same, fill with middle gray
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heightmap[x, y] = 128;
                }
            }
        }

        return heightmap;
    }

    /// <summary>
    /// Generates 2D Perlin noise at the given coordinates
    /// </summary>
    private double PerlinNoise2D(double x, double y)
    {
        // Find unit grid cell containing point
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        // Get relative coordinates within cell
        x -= Math.Floor(x);
        y -= Math.Floor(y);

        // Compute fade curves for x and y
        double u = Fade(x);
        double v = Fade(y);

        // Hash coordinates of the 4 cube corners
        int aa = _permutation[_permutation[X] + Y];
        int ab = _permutation[_permutation[X] + Y + 1];
        int ba = _permutation[_permutation[X + 1] + Y];
        int bb = _permutation[_permutation[X + 1] + Y + 1];

        // Blend results from 4 corners
        double x1 = Lerp(Grad(aa, x, y), Grad(ba, x - 1, y), u);
        double x2 = Lerp(Grad(ab, x, y - 1), Grad(bb, x - 1, y - 1), u);

        return Lerp(x1, x2, v);
    }

    /// <summary>
    /// Fade function for smooth interpolation (6t^5 - 15t^4 + 10t^3)
    /// </summary>
    private static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    /// <summary>
    /// Linear interpolation between two values
    /// </summary>
    private static double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    /// <summary>
    /// Gradient function that converts hash value to dot product with distance vector
    /// </summary>
    private static double Grad(int hash, double x, double y)
    {
        // Take the lower 4 bits of hash and use it to select a gradient direction
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    /// <summary>
    /// Generates a Perlin noise heightmap and saves it to a file as a PGM (Portable GrayMap) image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (PGM format)</param>
    /// <param name="scale">Scale factor for the noise (higher values = more zoomed in). Default is 0.05.</param>
    /// <param name="octaves">Number of noise layers to combine for fractal detail. Default is 4.</param>
    /// <param name="persistence">Amplitude decay factor per octave (0-1). Default is 0.5.</param>
    /// <param name="lacunarity">Frequency increase factor per octave. Default is 2.0.</param>
    public void GenerateAndSavePerlinNoise(int width, int height, string outputPath, double scale = 0.05, int octaves = 4, double persistence = 0.5, double lacunarity = 2.0)
    {
        var heightmap = GeneratePerlinNoise(width, height, scale, octaves, persistence, lacunarity);
        Utils.SaveAsPGM(heightmap, width, height, outputPath);
    }

    /// <summary>
    /// Generates a Perlin noise heightmap and saves it to a file as a BMP image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (BMP format)</param>
    /// <param name="scale">Scale factor for the noise (higher values = more zoomed in). Default is 0.05.</param>
    /// <param name="octaves">Number of noise layers to combine for fractal detail. Default is 4.</param>
    /// <param name="persistence">Amplitude decay factor per octave (0-1). Default is 0.5.</param>
    /// <param name="lacunarity">Frequency increase factor per octave. Default is 2.0.</param>
    public void GenerateAndSavePerlinNoiseAsBMP(int width, int height, string outputPath, double scale = 0.05, int octaves = 4, double persistence = 0.5, double lacunarity = 2.0)
    {
        var heightmap = GeneratePerlinNoise(width, height, scale, octaves, persistence, lacunarity);
        Utils.SaveAsBMP(heightmap, width, height, outputPath);
    }

    /// <summary>
    /// Generates a white noise heightmap and saves it to a file as a PGM (Portable GrayMap) image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (PGM format)</param>
    public void GenerateAndSaveWhiteNoise(int width, int height, string outputPath)
    {
        var heightmap = GenerateWhiteNoise(width, height);
        Utils.SaveAsPGM(heightmap, width, height, outputPath);
    }

    /// <summary>
    /// Generates a white noise heightmap and saves it to a file as a BMP image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (BMP format)</param>
    public void GenerateAndSaveWhiteNoiseAsBMP(int width, int height, string outputPath)
    {
        var heightmap = GenerateWhiteNoise(width, height);
        Utils.SaveAsBMP(heightmap, width, height, outputPath);
    }
}
