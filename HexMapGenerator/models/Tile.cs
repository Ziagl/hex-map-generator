using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Models;

public class Tile : HexTile
{
    public TerrainType terrain = TerrainType.SHALLOW_WATER;
    public LandscapeType landscape = LandscapeType.NONE;
    public RiverType river = RiverType.NONE;
    public int continentSeed = 0;
    public double elevation = 0.0;
}
