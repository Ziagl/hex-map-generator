using com.hexagonsimulations.HexMapHeightmap.Models;

namespace com.hexagonsimulations.HexMapHeightmap;

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
    /// <returns>A 2D double array containing normalized heightmap values (0.0-1.0)</returns>
    public double[,] GenerateWhiteNoise(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive values");
        }

        var heightmap = new double[width, height];

        // Generate white noise - random value normalized to 0.0-1.0 range
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heightmap[x, y] = _random.Next(0, 256) / 255.0;
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
    /// <returns>A 2D double array containing normalized heightmap values (0.0-1.0)</returns>
    public double[,] GeneratePerlinNoise(int width, int height, double scale = 0.05, int octaves = 4, double persistence = 0.5, double lacunarity = 2.0)
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

        // Normalize to 0.0-1.0 range
        double range = maxValue - minValue;
        if (range > 0)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseValues[x, y] = (noiseValues[x, y] - minValue) / range;
                }
            }
        }
        else
        {
            // If all values are the same, fill with 0.5
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseValues[x, y] = 0.5;
                }
            }
        }

        return noiseValues;
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
        double u = Utils.Fade(x);
        double v = Utils.Fade(y);

        // Hash coordinates of the 4 cube corners
        int aa = _permutation[_permutation[X] + Y];
        int ab = _permutation[_permutation[X] + Y + 1];
        int ba = _permutation[_permutation[X + 1] + Y];
        int bb = _permutation[_permutation[X + 1] + Y + 1];

        // Blend results from 4 corners
        double x1 = Utils.Lerp(Utils.Grad(aa, x, y), Utils.Grad(ba, x - 1, y), u);
        double x2 = Utils.Lerp(Utils.Grad(ab, x, y - 1), Utils.Grad(bb, x - 1, y - 1), u);

        return Utils.Lerp(x1, x2, v);
    }

    /// <summary>
    /// Generates an elliptic continent heightmap with the specified dimensions
    /// Creates a heightmap with an elliptical falloff from center, simulating a continental landmass
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="percentOfMap">Percentage of map covered by the ellipse (0.0-1.0). Default is 0.85.</param>
    /// <returns>A 2D double array containing normalized heightmap values (0.0-1.0)</returns>
    public double[,] GenerateEllipticContinent(int width, int height, double percentOfMap = 0.85)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive values");
        }

        if (percentOfMap <= 0 || percentOfMap > 1.0)
        {
            throw new ArgumentException("PercentOfMap must be between 0 and 1");
        }

        var heightmap = new double[width, height];
        
        // Calculate center offsets
        double xdim = width / 2.0;
        double ydim = height / 2.0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Convert to centered coordinates
                double xCentered = x - xdim;
                double yCentered = y - ydim;

                // Add randomness for variation
                double randomScale = _random.NextDouble();
                double ratio = percentOfMap + 0.1 * _random.NextDouble();

                // Calculate ellipse parameters
                double a = ratio * xdim;
                double b = ratio * ydim;

                // Calculate distance factor using elliptic formula
                double distanceFactor = 
                    (xCentered * xCentered) / (a * a) + 
                    (yCentered * yCentered) / (b * b) + 
                    Math.Pow(xCentered + yCentered, 2) / Math.Pow(a + b, 2);

                // Calculate height value with falloff
                double value = Math.Min(0.3, 1.0 - (5.0 * distanceFactor * distanceFactor + randomScale) / 3.0);
                
                // Clamp and normalize to 0.0-1.0 range
                heightmap[x, y] = Math.Max(0.0, Math.Min(1.0, value));
            }
        }

        return heightmap;
    }

    /// <summary>
    /// Blends two heightmaps together using a weighted average
    /// </summary>
    /// <param name="heightmap1">The first heightmap</param>
    /// <param name="heightmap2">The second heightmap</param>
    /// <param name="factor">Blending factor (0.0-1.0). A value of 0.75 means 75% of heightmap1 and 25% of heightmap2</param>
    /// <returns>A new blended heightmap</returns>
    /// <exception cref="ArgumentException">Thrown when heightmaps have different dimensions or factor is outside valid range</exception>
    public double[,] BlendHeightmaps(double[,] heightmap1, double[,] heightmap2, double factor)
    {
        if (heightmap1 == null)
        {
            throw new ArgumentException("First heightmap cannot be null");
        }

        if (heightmap2 == null)
        {
            throw new ArgumentException("Second heightmap cannot be null");
        }

        int width1 = heightmap1.GetLength(0);
        int height1 = heightmap1.GetLength(1);
        int width2 = heightmap2.GetLength(0);
        int height2 = heightmap2.GetLength(1);

        if (width1 != width2 || height1 != height2)
        {
            throw new ArgumentException($"Heightmap dimensions must match. First: {width1}x{height1}, Second: {width2}x{height2}");
        }

        if (factor < 0.0 || factor > 1.0)
        {
            throw new ArgumentException($"Factor must be between 0.0 and 1.0, got {factor}");
        }

        var blended = new double[width1, height1];
        //var normalized1 = Normalize(heightmap1, height1, width1);
        //var normalized2 = Normalize(heightmap2, height2, width2);
        double inverseFactor = 1.0 - factor;

        for (int y = 0; y < height1; y++)
        {
            for (int x = 0; x < width1; x++)
            {
                double value = (heightmap1[x, y] * factor) + (heightmap2[x, y] * inverseFactor);
                blended[x, y] = value;
            }
        }

        return blended;
    }



    private double[,] Normalize(double[,] heightmap, int height, int width)
    {

        // Normalize blended heightmap to 0.0-1.0 range
        double minValue = double.MaxValue;
        double maxValue = double.MinValue;

        // Find min and max values
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                minValue = Math.Min(minValue, heightmap[x, y]);
                maxValue = Math.Max(maxValue, heightmap[x, y]);
            }
        }

        // Normalize to 0.0-1.0 range
        double range = maxValue - minValue;
        if (range > 0)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heightmap[x, y] = (heightmap[x, y] - minValue) / range;
                }
            }
        }
        else
        {
            // If all values are the same, fill with 0.5
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heightmap[x, y] = 0.5;
                }
            }
        }

        return heightmap;
    }

    /// <summary>
    /// Clamps all values in a heightmap to the range [0.0, 1.0]
    /// </summary>
    /// <param name="heightmap">The heightmap to clamp</param>
    /// <param name="height">Height of the heightmap</param>
    /// <param name="width">Width of the heightmap</param>
    /// <returns>The clamped heightmap</returns>
    private double[,] Clamp(double[,] heightmap, int height, int width)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightmap[x, y] > 1.0)
                {
                    heightmap[x, y] = 1.0;
                }
                else if (heightmap[x, y] < 0.0)
                {
                    heightmap[x, y] = 0.0;
                }
            }
        }

        return heightmap;
    }
}
