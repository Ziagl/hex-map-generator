using HexMapGenerator.Models;

namespace HexMapGenerator.Interfaces;

internal interface IMapTerrainGenerator
{
    void Generate(MapData data);
}
