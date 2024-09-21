using HexMapGenerator.enums;

namespace HexMapGenerator.interfaces;

internal interface IMapLandscapeShaper
{
    (List<int> terrain, List<int> landscape, List<int> river, Dictionary<string, string> riverTileDirections) Generate(int[][] map, float factorRiver, int riverBed);
}
