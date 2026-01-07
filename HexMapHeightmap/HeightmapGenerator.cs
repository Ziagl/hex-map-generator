using HexMapHeightmap.Models;

namespace HexMapHeightmap;

/// <summary>
/// Generates heightmap images using various noise algorithms
/// </summary>
public class HeightmapGenerator
{
    private readonly Random _random;

    /// <summary>
    /// Creates a new HeightmapGenerator with an optional seed for reproducible results
    /// </summary>
    /// <param name="seed">Optional seed for random number generation. If null, a random seed is used.</param>
    public HeightmapGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
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

    // todo generate perlin noise

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
