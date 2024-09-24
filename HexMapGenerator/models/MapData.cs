using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;
using HexMapGenerator.enums;

namespace HexMapGenerator.models;

public record MapData
{
    public List<int> TerrainMap = new();
    public List<int> LandscapeMap = new();
    public List<int> RiverMap = new();
    public Dictionary<CubeCoordinates, List<Direction>> RiverTileDirections = new();
    public int Rows;
    public int Columns;
    public MapType Type = MapType.RANDOM;
    public MapSize Size = MapSize.MEDIUM;
    public MapTemperature Temperature = MapTemperature.NORMAL;
    public MapHumidity Humidity = MapHumidity.NORMAL;
}
