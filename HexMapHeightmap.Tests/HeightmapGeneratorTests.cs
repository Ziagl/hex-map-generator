namespace HexMapHeightmap.Tests;

/// <summary>
/// Unit tests for HeightmapGenerator that validate data without creating output files
/// </summary>
[TestClass]
public sealed class HeightmapGeneratorDataTests
{
    [TestMethod]
    public void GenerateWhiteNoise_ValidDimensions_ReturnsCorrectSize()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 50;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GenerateWhiteNoise_WithSeed_ProducesReproducibleResults()
    {
        // Arrange
        int seed = 12345;
        var generator1 = new HeightmapGenerator(seed);
        var generator2 = new HeightmapGenerator(seed);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GenerateWhiteNoise(width, height);
        var heightmap2 = generator2.GenerateWhiteNoise(width, height);

        // Assert
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Assert.AreEqual(heightmap1[x, y], heightmap2[x, y], $"Pixel mismatch at ({x}, {y})");
            }
        }
    }

    [TestMethod]
    public void GenerateWhiteNoise_WithDifferentSeeds_ProducesDifferentResults()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(111);
        var generator2 = new HeightmapGenerator(222);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GenerateWhiteNoise(width, height);
        var heightmap2 = generator2.GenerateWhiteNoise(width, height);

        // Assert - at least some pixels should be different
        bool foundDifference = false;
        for (int y = 0; y < height && !foundDifference; y++)
        {
            for (int x = 0; x < width && !foundDifference; x++)
            {
                if (heightmap1[x, y] != heightmap2[x, y])
                {
                    foundDifference = true;
                }
            }
        }
        Assert.IsTrue(foundDifference, "Different seeds should produce different heightmaps");
    }

    [TestMethod]
    public void GenerateWhiteNoise_AllValuesInValidRange()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 100;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = heightmap[x, y];
                Assert.IsTrue(value >= 0 && value <= 255, $"Value at ({x}, {y}) is {value}, expected 0-255");
            }
        }
    }

    [TestMethod]
    public void GenerateWhiteNoise_ProducesDistributedValues()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 200;
        int height = 200;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert - check that we have a reasonable distribution
        int[] histogram = new int[256];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                histogram[heightmap[x, y]]++;
            }
        }

        // At least 50% of possible values should appear at least once in a large map
        int usedValues = histogram.Count(count => count > 0);
        Assert.IsGreaterThan(128, usedValues, $"Only {usedValues} of 256 possible values were used - distribution may be poor");
    }

    [TestMethod]
    public void GenerateWhiteNoise_ZeroWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateWhiteNoise(0, 100));
    }

    [TestMethod]
    public void GenerateWhiteNoise_ZeroHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateWhiteNoise(100, 0));
    }

    [TestMethod]
    public void GenerateWhiteNoise_NegativeWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateWhiteNoise(-10, 100));
    }

    [TestMethod]
    public void GenerateWhiteNoise_NegativeHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateWhiteNoise(100, -10));
    }

    [TestMethod]
    public void GenerateWhiteNoise_SmallDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1;
        int height = 1;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert
        Assert.AreEqual(1, heightmap.GetLength(0));
        Assert.AreEqual(1, heightmap.GetLength(1));
        Assert.IsTrue(heightmap[0, 0] >= 0 && heightmap[0, 0] <= 255);
    }

    [TestMethod]
    public void GenerateWhiteNoise_LargeDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1024;
        int height = 1024;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GenerateWhiteNoise_AsymmetricDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 200;
        int height = 50;

        // Act
        var heightmap = generator.GenerateWhiteNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void Constructor_WithoutSeed_CreatesGenerator()
    {
        // Act
        var generator = new HeightmapGenerator();

        // Assert
        Assert.IsNotNull(generator);
    }

    [TestMethod]
    public void Constructor_WithSeed_CreatesGenerator()
    {
        // Act
        var generator = new HeightmapGenerator(42);

        // Assert
        Assert.IsNotNull(generator);
    }

    [TestMethod]
    public void GenerateWhiteNoise_MultipleCallsSameGenerator_ProducesDifferentResults()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator.GenerateWhiteNoise(width, height);
        var heightmap2 = generator.GenerateWhiteNoise(width, height);

        // Assert - should produce different results on subsequent calls
        bool foundDifference = false;
        for (int y = 0; y < height && !foundDifference; y++)
        {
            for (int x = 0; x < width && !foundDifference; x++)
            {
                if (heightmap1[x, y] != heightmap2[x, y])
                {
                    foundDifference = true;
                }
            }
        }
        Assert.IsTrue(foundDifference, "Multiple calls should produce different heightmaps");
    }
}

/// <summary>
/// Visual output tests that create actual heightmap image files for manual inspection
/// These tests write to C:\Temp folder
/// </summary>
[TestClass]
public sealed class HeightmapGeneratorVisualTests
{
    private static readonly string OutputFolder = @"C:\Temp\heightmap_tests";

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        // Ensure output directory exists
        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }
    }

    [TestMethod]
    public void GenerateAndSaveWhiteNoise_PGM_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_white_noise.pgm");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        generator.GenerateAndSaveWhiteNoise(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "PGM file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "PGM file is empty");

        Console.WriteLine($"White noise heightmap saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateAndSaveWhiteNoise_BMP_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_white_noise.bmp");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        generator.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "BMP file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "BMP file is empty");

        // BMP file should be at least 54 bytes (header) + image data
        int expectedMinSize = 54 + (width * height * 3);
        Assert.IsTrue(fileInfo.Length >= expectedMinSize,
            $"BMP file size ({fileInfo.Length}) is smaller than expected minimum ({expectedMinSize})");

        Console.WriteLine($"White noise heightmap (BMP) saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateMultipleSizes_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var sizes = new (int width, int height, string name)[]
        {
            (128, 128, "small"),
            (256, 256, "medium"),
            (512, 512, "large"),
            (1024, 1024, "huge")
        };

        // Act & Assert
        foreach (var (width, height, name) in sizes)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_white_noise_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            generator.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {outputPath} - {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GenerateWithDifferentSeeds_BMP_CreatesDistinctFiles()
    {
        // Arrange
        int width = 256;
        int height = 256;
        var seeds = new[] { 111, 222, 333, 444, 555 };

        // Act & Assert
        foreach (int seed in seeds)
        {
            var generator = new HeightmapGenerator(seed);
            string outputPath = Path.Combine(OutputFolder, $"test_white_noise_seed_{seed}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            generator.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for seed {seed} was not created");
            Console.WriteLine($"Seed {seed}: {outputPath}");
        }

        Console.WriteLine("\nCompare these files to verify different seeds produce different patterns.");
    }

    [TestMethod]
    public void GenerateAsymmetricHeightmaps_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(777);
        var dimensions = new (int width, int height, string name)[]
        {
            (512, 256, "wide"),
            (256, 512, "tall"),
            (800, 200, "panorama"),
            (200, 800, "portrait")
        };

        // Act & Assert
        foreach (var (width, height, name) in dimensions)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_white_noise_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            generator.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GenerateComparisonSet_BothFormats_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(42);
        int width = 256;
        int height = 256;
        string pgmPath = Path.Combine(OutputFolder, "test_comparison.pgm");
        string bmpPath = Path.Combine(OutputFolder, "test_comparison.bmp");

        // Clean up existing files
        if (File.Exists(pgmPath)) File.Delete(pgmPath);
        if (File.Exists(bmpPath)) File.Delete(bmpPath);

        // Act
        generator.GenerateAndSaveWhiteNoise(width, height, pgmPath);

        // Generate a new one with same seed for BMP
        generator = new HeightmapGenerator(42);
        generator.GenerateAndSaveWhiteNoiseAsBMP(width, height, bmpPath);

        // Assert
        Assert.IsTrue(File.Exists(pgmPath), "PGM file was not created");
        Assert.IsTrue(File.Exists(bmpPath), "BMP file was not created");

        var pgmInfo = new FileInfo(pgmPath);
        var bmpInfo = new FileInfo(bmpPath);

        Console.WriteLine($"PGM format: {pgmPath} - {pgmInfo.Length:N0} bytes");
        Console.WriteLine($"BMP format: {bmpPath} - {bmpInfo.Length:N0} bytes");
        Console.WriteLine("\nBoth files should display the same pattern (same seed used).");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Optional: Clean up old test files older than 1 day
        var outputDir = new DirectoryInfo(OutputFolder);
        if (outputDir.Exists)
        {
            var oldFiles = outputDir.GetFiles("test_*.bmp")
                .Concat(outputDir.GetFiles("test_*.pgm"))
                .Where(f => (DateTime.Now - f.LastWriteTime).TotalDays > 1);

            foreach (var file in oldFiles)
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
