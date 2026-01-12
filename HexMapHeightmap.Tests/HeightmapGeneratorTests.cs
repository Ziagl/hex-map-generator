namespace com.hexagonsimulations.HexMapHeightmap.Tests;

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
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Assert.AreEqual(heightmap1[x, y], heightmap2[x, y], epsilon, $"Pixel mismatch at ({x}, {y})");
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
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double value = heightmap[x, y];
                Assert.IsTrue(value >= 0.0 - epsilon && value <= 1.0 + epsilon, $"Value at ({x}, {y}) is {value}, expected 0.0-1.0");
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
        // For doubles in 0.0-1.0 range, we'll divide into 100 bins
        int binCount = 100;
        int[] histogram = new int[binCount];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double value = heightmap[x, y];
                int binIndex = Math.Min((int)(value * binCount), binCount - 1);
                histogram[binIndex]++;
            }
        }

        // At least 50% of bins should have values in a large map
        int usedBins = histogram.Count(count => count > 0);
        Assert.IsGreaterThan(binCount / 2, usedBins, $"Only {usedBins} of {binCount} bins were used - distribution may be poor");
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
        const double epsilon = 1e-10;
        Assert.AreEqual(1, heightmap.GetLength(0));
        Assert.AreEqual(1, heightmap.GetLength(1));
        Assert.IsTrue(heightmap[0, 0] >= 0.0 - epsilon && heightmap[0, 0] <= 1.0 + epsilon);
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

    // ===== Perlin Noise Tests =====

    [TestMethod]
    public void GeneratePerlinNoise_ValidDimensions_ReturnsCorrectSize()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 50;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GeneratePerlinNoise_WithSeed_ProducesReproducibleResults()
    {
        // Arrange
        int seed = 12345;
        var generator1 = new HeightmapGenerator(seed);
        var generator2 = new HeightmapGenerator(seed);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GeneratePerlinNoise(width, height);
        var heightmap2 = generator2.GeneratePerlinNoise(width, height);

        // Assert
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Assert.AreEqual(heightmap1[x, y], heightmap2[x, y], epsilon, $"Pixel mismatch at ({x}, {y})");
            }
        }
    }

    [TestMethod]
    public void GeneratePerlinNoise_WithDifferentSeeds_ProducesDifferentResults()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(111);
        var generator2 = new HeightmapGenerator(222);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GeneratePerlinNoise(width, height);
        var heightmap2 = generator2.GeneratePerlinNoise(width, height);

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
    public void GeneratePerlinNoise_AllValuesInValidRange()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 100;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double value = heightmap[x, y];
                Assert.IsTrue(value >= 0.0 - epsilon && value <= 1.0 + epsilon, $"Value at ({x}, {y}) is {value}, expected 0.0-1.0");
            }
        }
    }

    [TestMethod]
    public void GeneratePerlinNoise_ProducesSmoothGradients()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 100;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert - Perlin noise should have smooth gradients (not pure random like white noise)
        // Check that neighboring pixels don't differ too drastically
        int largeJumps = 0;
        int totalComparisons = 0;
        double jumpThreshold = 0.2; // 20% of range (was 50/255 ≈ 0.196)

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                double currentValue = heightmap[x, y];
                double rightValue = heightmap[x + 1, y];
                double downValue = heightmap[x, y + 1];

                if (Math.Abs(currentValue - rightValue) > jumpThreshold)
                {
                    largeJumps++;
                }
                if (Math.Abs(currentValue - downValue) > jumpThreshold)
                {
                    largeJumps++;
                }

                totalComparisons += 2;
            }
        }

        // Perlin noise should have fewer than 10% large jumps (white noise would have ~30-40%)
        double jumpPercentage = (double)largeJumps / totalComparisons;
        Assert.IsLessThan(0.1, jumpPercentage, 
            $"Too many large value jumps ({jumpPercentage:P2}) - noise may not be smooth enough");
    }

    [TestMethod]
    public void GeneratePerlinNoise_WithDifferentScales_ProducesDifferentPatterns()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(123);
        var generator2 = new HeightmapGenerator(123); // Same seed
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GeneratePerlinNoise(width, height, scale: 0.02); // Zoomed out
        var heightmap2 = generator2.GeneratePerlinNoise(width, height, scale: 0.1);  // Zoomed in

        // Assert - different scales should produce different patterns
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
        Assert.IsTrue(foundDifference, "Different scales should produce different heightmaps");
    }

    [TestMethod]
    public void GeneratePerlinNoise_WithDifferentOctaves_ProducesDifferentDetail()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(456);
        var generator2 = new HeightmapGenerator(456); // Same seed
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GeneratePerlinNoise(width, height, octaves: 1); // Low detail
        var heightmap2 = generator2.GeneratePerlinNoise(width, height, octaves: 6); // High detail

        // Assert - different octaves should produce different patterns
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
        Assert.IsTrue(foundDifference, "Different octaves should produce different heightmaps");
    }

    [TestMethod]
    public void GeneratePerlinNoise_ZeroWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(0, 100));
    }

    [TestMethod]
    public void GeneratePerlinNoise_ZeroHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 0));
    }

    [TestMethod]
    public void GeneratePerlinNoise_NegativeWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(-10, 100));
    }

    [TestMethod]
    public void GeneratePerlinNoise_NegativeHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, -10));
    }

    [TestMethod]
    public void GeneratePerlinNoise_ZeroScale_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, scale: 0));
    }

    [TestMethod]
    public void GeneratePerlinNoise_NegativeScale_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, scale: -0.5));
    }

    [TestMethod]
    public void GeneratePerlinNoise_ZeroOctaves_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, octaves: 0));
    }

    [TestMethod]
    public void GeneratePerlinNoise_NegativeOctaves_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, octaves: -1));
    }

    [TestMethod]
    public void GeneratePerlinNoise_PersistenceOutOfRange_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, persistence: -0.1));
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, persistence: 1.1));
    }

    [TestMethod]
    public void GeneratePerlinNoise_LacunarityBelowOne_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GeneratePerlinNoise(100, 100, lacunarity: 0.5));
    }

    [TestMethod]
    public void GeneratePerlinNoise_SmallDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1;
        int height = 1;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert
        const double epsilon = 1e-10;
        Assert.AreEqual(1, heightmap.GetLength(0));
        Assert.AreEqual(1, heightmap.GetLength(1));
        Assert.IsTrue(heightmap[0, 0] >= 0.0 - epsilon && heightmap[0, 0] <= 1.0 + epsilon);
    }

    [TestMethod]
    public void GeneratePerlinNoise_LargeDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1024;
        int height = 1024;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GeneratePerlinNoise_AsymmetricDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 200;
        int height = 50;

        // Act
        var heightmap = generator.GeneratePerlinNoise(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GeneratePerlinNoise_MultipleCallsSameGenerator_ProducesSameResults()
    {
        // Arrange - Note: Perlin noise is deterministic with same permutation table
        var generator = new HeightmapGenerator(789);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator.GeneratePerlinNoise(width, height);
        var heightmap2 = generator.GeneratePerlinNoise(width, height);

        // Assert - should produce same results because permutation table is fixed per instance
        bool allSame = true;
        for (int y = 0; y < height && allSame; y++)
        {
            for (int x = 0; x < width && allSame; x++)
            {
                if (heightmap1[x, y] != heightmap2[x, y])
                {
                    allSame = false;
                }
            }
        }
        Assert.IsTrue(allSame, "Same generator with same parameters should produce identical heightmaps");
    }

    // ===== Elliptic Continent Tests =====

    [TestMethod]
    public void GenerateEllipticContinent_ValidDimensions_ReturnsCorrectSize()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 50;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GenerateEllipticContinent_WithSeed_ProducesReproducibleResults()
    {
        // Arrange
        int seed = 12345;
        var generator1 = new HeightmapGenerator(seed);
        var generator2 = new HeightmapGenerator(seed);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GenerateEllipticContinent(width, height);
        var heightmap2 = generator2.GenerateEllipticContinent(width, height);

        // Assert
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Assert.AreEqual(heightmap1[x, y], heightmap2[x, y], epsilon, $"Pixel mismatch at ({x}, {y})");
            }
        }
    }

    [TestMethod]
    public void GenerateEllipticContinent_WithDifferentSeeds_ProducesDifferentResults()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(111);
        var generator2 = new HeightmapGenerator(222);
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GenerateEllipticContinent(width, height);
        var heightmap2 = generator2.GenerateEllipticContinent(width, height);

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
    public void GenerateEllipticContinent_AllValuesInValidRange()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 100;
        int height = 100;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert
        const double epsilon = 1e-10;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double value = heightmap[x, y];
                Assert.IsTrue(value >= 0.0 - epsilon && value <= 1.0 + epsilon, $"Value at ({x}, {y}) is {value}, expected 0.0-1.0");
            }
        }
    }

    [TestMethod]
    public void GenerateEllipticContinent_HasHigherValuesInCenter()
    {
        // Arrange
        var generator = new HeightmapGenerator(42);
        int width = 100;
        int height = 100;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert - center should generally have higher values than edges
        double centerValue = heightmap[width / 2, height / 2];
        double cornerValue = heightmap[0, 0];
        
        Assert.IsGreaterThan(cornerValue, centerValue, 
            "Center of elliptic continent should have higher elevation than corners");
    }

    [TestMethod]
    public void GenerateEllipticContinent_WithDifferentPercentages_ProducesDifferentPatterns()
    {
        // Arrange
        var generator1 = new HeightmapGenerator(123);
        var generator2 = new HeightmapGenerator(123); // Same seed
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator1.GenerateEllipticContinent(width, height, percentOfMap: 0.5);  // Smaller continent
        var heightmap2 = generator2.GenerateEllipticContinent(width, height, percentOfMap: 0.9);  // Larger continent

        // Assert - different percentages should produce different patterns
        bool foundDifference = false;
        for (int y = 0; y < height && !foundDifference; y++)
        {
            for (int x = 0; x < width && !foundDifference; x++)
            {
                if (Math.Abs(heightmap1[x, y] - heightmap2[x, y]) > 0.01)
                {
                    foundDifference = true;
                }
            }
        }
        Assert.IsTrue(foundDifference, "Different percentages should produce different heightmaps");
    }

    [TestMethod]
    public void GenerateEllipticContinent_ZeroWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(0, 100));
    }

    [TestMethod]
    public void GenerateEllipticContinent_ZeroHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(100, 0));
    }

    [TestMethod]
    public void GenerateEllipticContinent_NegativeWidth_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(-10, 100));
    }

    [TestMethod]
    public void GenerateEllipticContinent_NegativeHeight_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(100, -10));
    }

    [TestMethod]
    public void GenerateEllipticContinent_ZeroPercent_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(100, 100, percentOfMap: 0));
    }

    [TestMethod]
    public void GenerateEllipticContinent_NegativePercent_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(100, 100, percentOfMap: -0.5));
    }

    [TestMethod]
    public void GenerateEllipticContinent_PercentAboveOne_ThrowsException()
    {
        // Arrange
        var generator = new HeightmapGenerator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateEllipticContinent(100, 100, percentOfMap: 1.5));
    }

    [TestMethod]
    public void GenerateEllipticContinent_SmallDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1;
        int height = 1;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert
        const double epsilon = 1e-10;
        Assert.AreEqual(1, heightmap.GetLength(0));
        Assert.AreEqual(1, heightmap.GetLength(1));
        Assert.IsTrue(heightmap[0, 0] >= 0.0 - epsilon && heightmap[0, 0] <= 1.0 + epsilon);
    }

    [TestMethod]
    public void GenerateEllipticContinent_LargeDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 1024;
        int height = 1024;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GenerateEllipticContinent_AsymmetricDimensions_Works()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 200;
        int height = 50;

        // Act
        var heightmap = generator.GenerateEllipticContinent(width, height);

        // Assert
        Assert.AreEqual(width, heightmap.GetLength(0));
        Assert.AreEqual(height, heightmap.GetLength(1));
    }

    [TestMethod]
    public void GenerateEllipticContinent_MultipleCallsSameGenerator_ProducesDifferentResults()
    {
        // Arrange
        var generator = new HeightmapGenerator();
        int width = 50;
        int height = 50;

        // Act
        var heightmap1 = generator.GenerateEllipticContinent(width, height);
        var heightmap2 = generator.GenerateEllipticContinent(width, height);

        // Assert - should produce different results due to randomness
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
        Assert.IsTrue(foundDifference, "Multiple calls should produce different heightmaps due to randomness");
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
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_white_noise.pgm");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSaveWhiteNoise(width, height, outputPath);

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
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_white_noise.bmp");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "BMP file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "BMP file is empty");

        // BMP file should be at least 54 bytes (header) + image data
        long expectedMinSize = 54 + (width * height * 3);
        Assert.IsGreaterThanOrEqualTo(expectedMinSize, fileInfo.Length,
            $"BMP file size ({fileInfo.Length}) is smaller than expected minimum ({expectedMinSize})");

        Console.WriteLine($"White noise heightmap (BMP) saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateMultipleSizes_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var saver = new HeightmapExporter(generator);
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

            saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

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
            var saver = new HeightmapExporter(generator);
            string outputPath = Path.Combine(OutputFolder, $"test_white_noise_seed_{seed}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

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
        var saver = new HeightmapExporter(generator);
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

            saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, outputPath);

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
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        string pgmPath = Path.Combine(OutputFolder, "test_comparison.pgm");
        string bmpPath = Path.Combine(OutputFolder, "test_comparison.bmp");

        // Clean up existing files
        if (File.Exists(pgmPath)) File.Delete(pgmPath);
        if (File.Exists(bmpPath)) File.Delete(bmpPath);

        // Act
        saver.GenerateAndSaveWhiteNoise(width, height, pgmPath);

        // Generate a new one with same seed for BMP
        generator = new HeightmapGenerator(42);
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, bmpPath);

        // Assert
        Assert.IsTrue(File.Exists(pgmPath), "PGM file was not created");
        Assert.IsTrue(File.Exists(bmpPath), "BMP file was not created");

        var pgmInfo = new FileInfo(pgmPath);
        var bmpInfo = new FileInfo(bmpPath);

        Console.WriteLine($"PGM format: {pgmPath} - {pgmInfo.Length:N0} bytes");
        Console.WriteLine($"BMP format: {bmpPath} - {bmpInfo.Length:N0} bytes");
        Console.WriteLine("\nBoth files should display the same pattern (same seed used).");
    }

    // ===== Perlin Noise Visual Tests =====

    [TestMethod]
    public void GenerateAndSavePerlinNoise_PGM_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_perlin_noise.pgm");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSavePerlinNoise(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "PGM file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "PGM file is empty");

        Console.WriteLine($"Perlin noise heightmap saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateAndSavePerlinNoise_BMP_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_perlin_noise.bmp");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "BMP file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "BMP file is empty");

        // BMP file should be at least 54 bytes (header) + image data
        long expectedMinSize = 54 + (width * height * 3);
        Assert.IsGreaterThanOrEqualTo(expectedMinSize, fileInfo.Length,
            $"BMP file size ({fileInfo.Length}) is smaller than expected minimum ({expectedMinSize})");

        Console.WriteLine($"Perlin noise heightmap (BMP) saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GeneratePerlinMultipleSizes_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var saver = new HeightmapExporter(generator);
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
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {outputPath} - {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GeneratePerlinWithDifferentSeeds_BMP_CreatesDistinctFiles()
    {
        // Arrange
        int width = 256;
        int height = 256;
        var seeds = new[] { 111, 222, 333, 444, 555 };

        // Act & Assert
        foreach (int seed in seeds)
        {
            var generator = new HeightmapGenerator(seed);
            var saver = new HeightmapExporter(generator);
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_seed_{seed}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for seed {seed} was not created");
            Console.WriteLine($"Seed {seed}: {outputPath}");
        }

        Console.WriteLine("\nCompare these files to verify different seeds produce different patterns.");
    }

    [TestMethod]
    public void GeneratePerlinWithDifferentScales_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(777);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        var scales = new[] { 0.01, 0.03, 0.05, 0.1, 0.2 };

        // Act & Assert
        foreach (double scale in scales)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_scale_{scale:F2}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath, scale: scale);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for scale {scale} was not created");
            Console.WriteLine($"Scale {scale:F2}: {outputPath}");
        }

        Console.WriteLine("\nLower scales = zoomed out (larger features), higher scales = zoomed in (smaller features)");
    }

    [TestMethod]
    public void GeneratePerlinWithDifferentOctaves_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(888);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        var octaves = new[] { 1, 2, 4, 6, 8 };

        // Act & Assert
        foreach (int octave in octaves)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_octaves_{octave}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath, octaves: octave);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {octave} octaves was not created");
            Console.WriteLine($"Octaves {octave}: {outputPath}");
        }

        Console.WriteLine("\nMore octaves = more detail/texture");
    }

    [TestMethod]
    public void GeneratePerlinWithDifferentPersistence_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(321);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        var persistenceValues = new[] { 0.2, 0.4, 0.5, 0.6, 0.8 };

        // Act & Assert
        foreach (double persistence in persistenceValues)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_persistence_{persistence:F1}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath, persistence: persistence);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for persistence {persistence} was not created");
            Console.WriteLine($"Persistence {persistence:F1}: {outputPath}");
        }

        Console.WriteLine("\nLower persistence = smoother, higher persistence = rougher/more detail influence");
    }

    [TestMethod]
    public void GeneratePerlinAsymmetricHeightmaps_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(654);
        var saver = new HeightmapExporter(generator);
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
            string outputPath = Path.Combine(OutputFolder, $"test_perlin_noise_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSavePerlinNoiseAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GeneratePerlinComparisonSet_BothFormats_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(42);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        string pgmPath = Path.Combine(OutputFolder, "test_perlin_comparison.pgm");
        string bmpPath = Path.Combine(OutputFolder, "test_perlin_comparison.bmp");

        // Clean up existing files
        if (File.Exists(pgmPath)) File.Delete(pgmPath);
        if (File.Exists(bmpPath)) File.Delete(bmpPath);

        // Act
        saver.GenerateAndSavePerlinNoise(width, height, pgmPath);

        // Generate a new one with same seed for BMP
        generator = new HeightmapGenerator(42);
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSavePerlinNoiseAsBMP(width, height, bmpPath);

        // Assert
        Assert.IsTrue(File.Exists(pgmPath), "PGM file was not created");
        Assert.IsTrue(File.Exists(bmpPath), "BMP file was not created");

        var pgmInfo = new FileInfo(pgmPath);
        var bmpInfo = new FileInfo(bmpPath);

        Console.WriteLine($"PGM format: {pgmPath} - {pgmInfo.Length:N0} bytes");
        Console.WriteLine($"BMP format: {bmpPath} - {bmpInfo.Length:N0} bytes");
        Console.WriteLine("\nBoth files should display the same pattern (same seed used).");
    }

    [TestMethod]
    public void GenerateNoiseComparison_WhiteVsPerlin_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string whiteNoisePath = Path.Combine(OutputFolder, "comparison_white_noise.bmp");
        string perlinNoisePath = Path.Combine(OutputFolder, "comparison_perlin_noise.bmp");

        // Clean up existing files
        if (File.Exists(whiteNoisePath)) File.Delete(whiteNoisePath);
        if (File.Exists(perlinNoisePath)) File.Delete(perlinNoisePath);

        // Act
        saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, whiteNoisePath);
        generator = new HeightmapGenerator(999); // Reset to same seed
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSavePerlinNoiseAsBMP(width, height, perlinNoisePath);

        // Assert
        Assert.IsTrue(File.Exists(whiteNoisePath), "White noise file was not created");
        Assert.IsTrue(File.Exists(perlinNoisePath), "Perlin noise file was not created");

        Console.WriteLine($"White Noise: {whiteNoisePath}");
        Console.WriteLine($"Perlin Noise: {perlinNoisePath}");
        Console.WriteLine("\nCompare these files to see the difference between white (random) and Perlin (smooth) noise.");
    }

    // ===== Elliptic Continent Visual Tests =====

    [TestMethod]
    public void GenerateAndSaveEllipticContinent_PGM_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_elliptic_continent.pgm");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSaveEllipticContinent(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "PGM file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "PGM file is empty");

        Console.WriteLine($"Elliptic continent heightmap saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateAndSaveEllipticContinent_BMP_CreatesFile()
    {
        // Arrange
        var generator = new HeightmapGenerator(12345);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string outputPath = Path.Combine(OutputFolder, "test_elliptic_continent.bmp");

        // Clean up any existing file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        // Act
        saver.GenerateAndSaveEllipticContinentAsBMP(width, height, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "BMP file was not created");
        var fileInfo = new FileInfo(outputPath);
        Assert.IsGreaterThan(0, fileInfo.Length, "BMP file is empty");

        // BMP file should be at least 54 bytes (header) + image data
        long expectedMinSize = 54 + (width * height * 3);
        Assert.IsGreaterThanOrEqualTo(expectedMinSize, fileInfo.Length,
            $"BMP file size ({fileInfo.Length}) is smaller than expected minimum ({expectedMinSize})");

        Console.WriteLine($"Elliptic continent heightmap (BMP) saved to: {outputPath}");
        Console.WriteLine($"File size: {fileInfo.Length:N0} bytes");
    }

    [TestMethod]
    public void GenerateEllipticMultipleSizes_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var saver = new HeightmapExporter(generator);
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
            string outputPath = Path.Combine(OutputFolder, $"test_elliptic_continent_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSaveEllipticContinentAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {outputPath} - {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GenerateEllipticWithDifferentSeeds_BMP_CreatesDistinctFiles()
    {
        // Arrange
        int width = 256;
        int height = 256;
        var seeds = new[] { 111, 222, 333, 444, 555 };

        // Act & Assert
        foreach (int seed in seeds)
        {
            var generator = new HeightmapGenerator(seed);
            var saver = new HeightmapExporter(generator);
            string outputPath = Path.Combine(OutputFolder, $"test_elliptic_continent_seed_{seed}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSaveEllipticContinentAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for seed {seed} was not created");
            Console.WriteLine($"Seed {seed}: {outputPath}");
        }

        Console.WriteLine("\nCompare these files to verify different seeds produce different patterns.");
    }

    [TestMethod]
    public void GenerateEllipticWithDifferentPercentages_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(777);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        var percentages = new[] { 0.5, 0.65, 0.75, 0.85, 0.95 };

        // Act & Assert
        foreach (double percentage in percentages)
        {
            string outputPath = Path.Combine(OutputFolder, $"test_elliptic_continent_percent_{percentage:F2}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSaveEllipticContinentAsBMP(width, height, outputPath, percentOfMap: percentage);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for percentage {percentage} was not created");
            Console.WriteLine($"Percentage {percentage:F2}: {outputPath}");
        }

        Console.WriteLine("\nLower percentages = smaller continent, higher percentages = larger continent coverage");
    }

    [TestMethod]
    public void GenerateEllipticAsymmetricHeightmaps_BMP_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(654);
        var saver = new HeightmapExporter(generator);
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
            string outputPath = Path.Combine(OutputFolder, $"test_elliptic_continent_{name}_{width}x{height}.bmp");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            saver.GenerateAndSaveEllipticContinentAsBMP(width, height, outputPath);

            Assert.IsTrue(File.Exists(outputPath), $"BMP file for {name} was not created");
            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"{name} ({width}×{height}): {fileInfo.Length:N0} bytes");
        }
    }

    [TestMethod]
    public void GenerateEllipticComparisonSet_BothFormats_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(42);
        var saver = new HeightmapExporter(generator);
        int width = 256;
        int height = 256;
        string pgmPath = Path.Combine(OutputFolder, "test_elliptic_comparison.pgm");
        string bmpPath = Path.Combine(OutputFolder, "test_elliptic_comparison.bmp");

        // Clean up existing files
        if (File.Exists(pgmPath)) File.Delete(pgmPath);
        if (File.Exists(bmpPath)) File.Delete(bmpPath);

        // Act
        saver.GenerateAndSaveEllipticContinent(width, height, pgmPath);

        // Generate a new one with same seed for BMP
        generator = new HeightmapGenerator(42);
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSaveEllipticContinentAsBMP(width, height, bmpPath);

        // Assert
        Assert.IsTrue(File.Exists(pgmPath), "PGM file was not created");
        Assert.IsTrue(File.Exists(bmpPath), "BMP file was not created");

        var pgmInfo = new FileInfo(pgmPath);
        var bmpInfo = new FileInfo(bmpPath);

        Console.WriteLine($"PGM format: {pgmPath} - {pgmInfo.Length:N0} bytes");
        Console.WriteLine($"BMP format: {bmpPath} - {bmpInfo.Length:N0} bytes");
        Console.WriteLine("\nBoth files should display the same pattern (same seed used).");
    }

    [TestMethod]
    public void GenerateAllNoiseTypes_Comparison_CreatesFiles()
    {
        // Arrange
        var generator = new HeightmapGenerator(999);
        var saver = new HeightmapExporter(generator);
        int width = 512;
        int height = 512;
        string whiteNoisePath = Path.Combine(OutputFolder, "comparison_all_white_noise.bmp");
        string perlinNoisePath = Path.Combine(OutputFolder, "comparison_all_perlin_noise.bmp");
        string ellipticContinentPath = Path.Combine(OutputFolder, "comparison_all_elliptic_continent.bmp");

        // Clean up existing files
        if (File.Exists(whiteNoisePath)) File.Delete(whiteNoisePath);
        if (File.Exists(perlinNoisePath)) File.Delete(perlinNoisePath);
        if (File.Exists(ellipticContinentPath)) File.Delete(ellipticContinentPath);

        // Act
        saver.GenerateAndSaveWhiteNoiseAsBMP(width, height, whiteNoisePath);
        generator = new HeightmapGenerator(999); // Reset to same seed
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSavePerlinNoiseAsBMP(width, height, perlinNoisePath);
        generator = new HeightmapGenerator(999); // Reset to same seed
        saver = new HeightmapExporter(generator);
        saver.GenerateAndSaveEllipticContinentAsBMP(width, height, ellipticContinentPath);

        // Assert
        Assert.IsTrue(File.Exists(whiteNoisePath), "White noise file was not created");
        Assert.IsTrue(File.Exists(perlinNoisePath), "Perlin noise file was not created");
        Assert.IsTrue(File.Exists(ellipticContinentPath), "Elliptic continent file was not created");

        Console.WriteLine($"White Noise: {whiteNoisePath}");
        Console.WriteLine($"Perlin Noise: {perlinNoisePath}");
        Console.WriteLine($"Elliptic Continent: {ellipticContinentPath}");
        Console.WriteLine("\nCompare these files to see the difference between noise types:");
        Console.WriteLine("- White Noise: Pure random values");
        Console.WriteLine("- Perlin Noise: Smooth gradient noise");
        Console.WriteLine("- Elliptic Continent: Centered elliptical falloff simulating a landmass");
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
