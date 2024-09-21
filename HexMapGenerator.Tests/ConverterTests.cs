using HexMapGenerator.enums;

namespace HexMapGenerator.Tests;

public class ConverterTests
{
    [Fact]
    public void TestGenerateTiledJson()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.HIGHLAND, MapSize.SMALL, MapTemperature.NORMAL, MapHumidity.NORMAL, 0.0f);
        var converter = new Converter();
        string json = converter.GenerateTiledJson(generator.MapData, "tileset.png", 32, 34, 1536, 34, 48, 48, "#ffffff");
        Assert.True(json.Length > 5);
    }
}
