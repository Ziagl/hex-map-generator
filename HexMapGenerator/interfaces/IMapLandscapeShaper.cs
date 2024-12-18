using com.hexagonsimulations.HexMapGenerator.Models;

namespace com.hexagonsimulations.HexMapGenerator.Interfaces;

internal interface IMapLandscapeShaper
{
    void Generate(MapData data, float factorRiver, int riverBed);
}
