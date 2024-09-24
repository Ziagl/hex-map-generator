using HexMapGenerator.models;

namespace HexMapGenerator.interfaces;

internal interface IMapTerrainGenerator
{
    void Generate(MapData data);
}
