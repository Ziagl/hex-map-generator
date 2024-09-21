using HexMapGenerator.enums;

namespace HexMapGenerator.interfaces;

internal interface IMapTerrainGenerator
{
    int Rows { get; }
    int Columns { get; }

    List<int> Generate(MapSize size);
}
