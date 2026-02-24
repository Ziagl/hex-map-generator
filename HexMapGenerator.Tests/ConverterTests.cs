using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class ConverterTests
{
    [TestMethod]
    public void TestGenerateTiledJson()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.SUPER_CONTINENT, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.NORMAL, 0.0f);
        var converter = new Converter();
        string json = converter.GenerateTiledJson(generator.MapData, "tileset.png", 32, 34, 1536, 34, 48, 48, "#ffffff");
        File.WriteAllText(@"C:\Temp\tiled.json", json);
        Assert.IsGreaterThan(5, json.Length);
    }
}
