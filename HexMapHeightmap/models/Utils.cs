namespace com.hexagonsimulations.HexMapHeightmap.Models;

internal class Utils
{
    /// <summary>
    /// Saves a heightmap as a PGM (Portable GrayMap) image file
    /// PGM is a simple grayscale image format that can be opened by most image viewers
    /// </summary>
    internal static void SaveAsPGM(double[,] heightmap, int width, int height, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);

        // PGM header
        writer.WriteLine("P2");
        writer.WriteLine($"{width} {height}");
        writer.WriteLine("255");

        // Write pixel data, scaling from 0.0-1.0 to 0-255
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = (byte)(heightmap[x, y] * 255);
                writer.Write(value);
                if (x < width - 1)
                    writer.Write(" ");
            }
            writer.WriteLine();
        }
    }


    /// <summary>
    /// Saves a heightmap as a BMP (Bitmap) image file
    /// Creates a 24-bit RGB BMP with grayscale values
    /// </summary>
    internal static void SaveAsBMP(double[,] heightmap, int width, int height, string outputPath)
    {
        int rowSize = ((width * 3 + 3) / 4) * 4; // BMP rows must be padded to 4-byte boundary
        int imageSize = rowSize * height;
        int fileSize = 54 + imageSize; // 54 bytes for BMP headers

        using var stream = new FileStream(outputPath, FileMode.Create);
        using var writer = new BinaryWriter(stream);

        // BMP File Header (14 bytes)
        writer.Write((ushort)0x4D42); // "BM" signature
        writer.Write(fileSize);
        writer.Write(0); // Reserved
        writer.Write(54); // Pixel data offset

        // DIB Header (40 bytes - BITMAPINFOHEADER)
        writer.Write(40); // Header size
        writer.Write(width);
        writer.Write(height);
        writer.Write((ushort)1); // Color planes
        writer.Write((ushort)24); // Bits per pixel (24-bit RGB)
        writer.Write(0); // No compression
        writer.Write(imageSize);
        writer.Write(2835); // Horizontal resolution (pixels/meter)
        writer.Write(2835); // Vertical resolution (pixels/meter)
        writer.Write(0); // Colors in palette
        writer.Write(0); // Important colors

        // Pixel data (BGR format, bottom-to-top), scaling from 0.0-1.0 to 0-255
        byte[] padding = new byte[rowSize - (width * 3)];
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = (byte)(heightmap[x, y] * 255);
                writer.Write(value); // Blue
                writer.Write(value); // Green
                writer.Write(value); // Red
            }
            writer.Write(padding); // Row padding
        }
    }

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
