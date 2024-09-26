using com.hexagonsimulations.Geometry.Hex;
using HexMapGenerator.Enums;

namespace HexMapGenerator.Models;

public class Tile : HexTile
{
    public TerrainType terrain = TerrainType.SHALLOW_WATER;
    public LandscapeType landscape = LandscapeType.NONE;
    public RiverType river = RiverType.NONE;
    public int continentSeed = 0;
}
