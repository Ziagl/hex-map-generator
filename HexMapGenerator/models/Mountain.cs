using com.hexagonsimulations.Geometry.Hex;

namespace HexMapGenerator.models;

internal record Mountain
{
    public CubeCoordinates coordinates;
    public int distanceToWater = -1;    // distance to next water tile (const)
    public int distanceToRiver = -1;    // distance to next river tile (may change for each new river)
}
