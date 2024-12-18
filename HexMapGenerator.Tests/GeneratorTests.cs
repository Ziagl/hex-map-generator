using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class GeneratorTests
{
    [TestMethod]
    public void TestRandomGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.RANDOM, MapSize.TINY, MapTemperature.HOT, MapHumidity.DRY, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.Print().Length > 0);
    }

    [TestMethod]
    public void TestArchipelagoGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.ARCHIPELAGO, MapSize.TINY, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestContinentsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.CONTINENTS, MapSize.TINY, MapTemperature.HOT, MapHumidity.DRY, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestContinentIslandsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.CONTINENTS_ISLANDS, MapSize.TINY, MapTemperature.COLD, MapHumidity.DRY, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestHighlandGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.HIGHLAND, MapSize.SMALL, MapTemperature.NORMAL, MapHumidity.NORMAL, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestInlandSeaGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.INLAND_SEA, MapSize.MEDIUM, MapTemperature.HOT, MapHumidity.WET, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestIslandsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.ISLANDS, MapSize.LARGE, MapTemperature.NORMAL, MapHumidity.WET, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestLakesGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.LAKES, MapSize.HUGE, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestSmallContinentsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.SMALL_CONTINENTS, MapSize.MICRO, MapTemperature.HOT, MapHumidity.NORMAL, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestSuperContinentGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.SUPER_CONTINENT, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.DRY, 0.0f);
        Assert.AreEqual(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.AreEqual(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count == 0);
    }

    [TestMethod]
    public void TestRiver()
    {
        var generator = new Generator(1234);
        generator.GenerateMap(MapType.SUPER_CONTINENT, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.NORMAL, 2.0f);
        Assert.IsTrue(generator.MapData.RiverTileDirections.Count > 0);
        foreach (var directions in generator.MapData.RiverTileDirections)
        {
            Assert.IsTrue(directions.Value.Count > 0);
        }
    }
}
