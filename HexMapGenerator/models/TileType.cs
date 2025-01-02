using com.hexagonsimulations.HexMapGenerator.Enums;

namespace com.hexagonsimulations.HexMapGenerator.Models;

// fully describes single tile of map
public record TileType
{
    public TerrainType terrainType;
    public LandscapeType landscapeType;
    public RiverType riverType;
}
