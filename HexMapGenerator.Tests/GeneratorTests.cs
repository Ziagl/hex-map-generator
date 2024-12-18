using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

public class GeneratorTests
{
    [Fact]
    public void TestRandomGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.RANDOM, MapSize.TINY, MapTemperature.HOT, MapHumidity.DRY, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.Print().Length > 0);
    }

    [Fact]
    public void TestArchipelagoGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.ARCHIPELAGO, MapSize.TINY, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestContinentsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.CONTINENTS, MapSize.TINY, MapTemperature.HOT, MapHumidity.DRY, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestContinentIslandsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.CONTINENTS_ISLANDS, MapSize.TINY, MapTemperature.COLD, MapHumidity.DRY, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestHighlandGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.HIGHLAND, MapSize.SMALL, MapTemperature.NORMAL, MapHumidity.NORMAL, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestInlandSeaGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.INLAND_SEA, MapSize.MEDIUM, MapTemperature.HOT, MapHumidity.WET, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestIslandsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.ISLANDS, MapSize.LARGE, MapTemperature.NORMAL, MapHumidity.WET, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestLakesGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.LAKES, MapSize.HUGE, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestSmallContinentsGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.SMALL_CONTINENTS, MapSize.MICRO, MapTemperature.HOT, MapHumidity.NORMAL, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestSuperContinentGenerator()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.SUPER_CONTINENT, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.DRY, 0.0f);
        Assert.Equal(generator.MapData.TerrainMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.LandscapeMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.Equal(generator.MapData.RiverMap.Count, generator.MapData.Rows * generator.MapData.Columns);
        Assert.True(generator.MapData.RiverTileDirections.Count == 0);
    }

    [Fact]
    public void TestRiver()
    {
        var generator = new Generator(1234);
        generator.GenerateMap(MapType.SUPER_CONTINENT, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.NORMAL, 2.0f);
        Assert.True(generator.MapData.RiverTileDirections.Count  > 0);
        foreach(var directions in generator.MapData.RiverTileDirections)
        {
            Assert.True(directions.Value.Count > 0);
        }
    }
}
