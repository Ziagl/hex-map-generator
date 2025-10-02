using com.hexagonsimulations.HexMapBase.Enums;
using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Models;

public record MapData
{
    // Tile index maps (row-major, size Rows * Columns)
    public List<int> TerrainMap { get; set; } = new();
    public List<int> LandscapeMap { get; set; } = new();
    public List<int> RiverMap { get; set; } = new();

    // River outgoing edge directions per cube coordinate
    public Dictionary<CubeCoordinates, List<Direction>> RiverTileDirections { get; set; } = new();

    public int Rows { get; set; }
    public int Columns { get; set; }

    public MapType Type { get; set; } = MapType.RANDOM;
    public MapSize Size { get; set; } = MapSize.MEDIUM;
    public MapTemperature Temperature { get; set; } = MapTemperature.NORMAL;
    public MapHumidity Humidity { get; set; } = MapHumidity.NORMAL;
}
