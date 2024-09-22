using HexMapGenerator.enums;
using HexMapGenerator.models;

namespace HexMapGenerator.interfaces;

internal interface IMapLandscapeShaper
{
    void Generate(MapData data, MapTemperature temperature, MapHumidity humidity, float factorRiver, int riverBed);
}
