using HexMapGenerator.Models;

namespace HexMapGenerator.Interfaces;

internal interface IMapLandscapeShaper
{
    void Generate(MapData data, float factorRiver, int riverBed);
}
