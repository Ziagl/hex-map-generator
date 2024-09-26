using com.hexagonsimulations.Geometry.Hex;

namespace HexMapGenerator.Models;

internal class Mountain : HexTile
{
    public int distanceToWater = -1;    // distance to next water tile (const)
    public int distanceToRiver = -1;    // distance to next river tile (may change for each new river)
}
