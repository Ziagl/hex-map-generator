using HexMapGenerator.models;

namespace HexMapGenerator.interfaces;

internal interface IMapLandscapeShaper
{
    void Generate(MapData data, float factorRiver, int riverBed);
}
