using HexMapHeightmap.Models;

namespace HexMapHeightmap;

/// <summary>
/// Provides methods to generate and save heightmaps to files in various formats
/// </summary>
public class HeightmapExporter
{
    private readonly HeightmapGenerator _generator;

    /// <summary>
    /// Creates a new HeightmapExporter with the specified generator
    /// </summary>
    /// <param name="generator">The HeightmapGenerator to use for generating heightmaps</param>
    public HeightmapExporter(HeightmapGenerator generator)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    /// <summary>
    /// Generates a white noise heightmap and saves it to a file as a PGM (Portable GrayMap) image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (PGM format)</param>
    public void GenerateAndSaveWhiteNoise(int width, int height, string outputPath)
    {
        var heightmap = _generator.GenerateWhiteNoise(width, height);
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
        var heightmap = _generator.GenerateWhiteNoise(width, height);
        Utils.SaveAsBMP(heightmap, width, height, outputPath);
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
        var heightmap = _generator.GeneratePerlinNoise(width, height, scale, octaves, persistence, lacunarity);
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
        var heightmap = _generator.GeneratePerlinNoise(width, height, scale, octaves, persistence, lacunarity);
        Utils.SaveAsBMP(heightmap, width, height, outputPath);
    }

    /// <summary>
    /// Generates an elliptic continent heightmap and saves it to a file as a PGM (Portable GrayMap) image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (PGM format)</param>
    /// <param name="percentOfMap">Percentage of map covered by the ellipse (0.0-1.0). Default is 0.85.</param>
    public void GenerateAndSaveEllipticContinent(int width, int height, string outputPath, double percentOfMap = 0.85)
    {
        var heightmap = _generator.GenerateEllipticContinent(width, height, percentOfMap);
        Utils.SaveAsPGM(heightmap, width, height, outputPath);
    }

    /// <summary>
    /// Generates an elliptic continent heightmap and saves it to a file as a BMP image
    /// </summary>
    /// <param name="width">Width of the heightmap in pixels</param>
    /// <param name="height">Height of the heightmap in pixels</param>
    /// <param name="outputPath">Path where the heightmap image will be saved (BMP format)</param>
    /// <param name="percentOfMap">Percentage of map covered by the ellipse (0.0-1.0). Default is 0.85.</param>
    public void GenerateAndSaveEllipticContinentAsBMP(int width, int height, string outputPath, double percentOfMap = 0.85)
    {
        var heightmap = _generator.GenerateEllipticContinent(width, height, percentOfMap);
        Utils.SaveAsBMP(heightmap, width, height, outputPath);
    }
}
