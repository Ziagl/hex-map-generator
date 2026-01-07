namespace HexMapHeightmap.Models;

internal class Utils
{
    /// <summary>
    /// Saves a heightmap as a PGM (Portable GrayMap) image file
    /// PGM is a simple grayscale image format that can be opened by most image viewers
    /// </summary>
    internal static void SaveAsPGM(byte[,] heightmap, int width, int height, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);

        // PGM header
        writer.WriteLine("P2");
        writer.WriteLine($"{width} {height}");
        writer.WriteLine("255");

        // Write pixel data
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                writer.Write(heightmap[x, y]);
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
    internal static void SaveAsBMP(byte[,] heightmap, int width, int height, string outputPath)
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

        // Pixel data (BGR format, bottom-to-top)
        byte[] padding = new byte[rowSize - (width * 3)];
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = heightmap[x, y];
                writer.Write(value); // Blue
                writer.Write(value); // Green
                writer.Write(value); // Red
            }
            writer.Write(padding); // Row padding
        }
    }
}
