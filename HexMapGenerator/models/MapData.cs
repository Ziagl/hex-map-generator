using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;

namespace HexMapGenerator.models;

public record MapData
{
    public List<int> TerrainMap = new();
    public List<int> LandscapeMap = new();
    public List<int> RiverMap = new();
    public Dictionary<CubeCoordinates, List<Direction>> riverTileDirections = new();
    public int Rows;
    public int Columns;
}
