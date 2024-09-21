using com.hexagonsimulations.Geometry.Hex;
using HexMapGenerator.enums;

namespace HexMapGenerator.models;

public record Tile
{
    public CubeCoordinates coordinates = new(0, 0, 0);
    public TerrainType terrain = TerrainType.SHALLOW_WATER;
    public LandscapeType landscape = LandscapeType.NONE;
    public RiverType river = RiverType.NONE;
    public int continentSeed = 0;
}
