using HexMapGenerator.enums;
using HexMapGenerator.generators;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;
using HexMapGenerator.shapers;

namespace HexMapGenerator;

public class Generator
{
    private readonly string[] _layers = { "terrain", "landscape", "river" };
    private readonly int _riverbed = 3;
    private MapData _map;
    private Dictionary<string, string[]/*Direction*/> _mapRiverTileDirections;

    public Generator()
    {
        this._map = new();
        this._mapRiverTileDirections = new();
    }

    /// <summary>
    /// generate a map of given type and size
    /// </summary>
    /// <param name="type">type of map</param>
    /// <param name="size">size of map</param>
    /// <param name="temperature">temperature of map</param>
    /// <param name="humidity">humidity of map</param>
    /// <param name="factorRiver">factor of rivers to create (factor * Map.Size)</param>
    public void GenerateMap(MapType type, MapSize size, MapTemperature temperature, MapHumidity humidity, float factorRiver)
    {
        IMapTerrainGenerator generator = null;
        IMapLandscapeShaper shaper = new DefaultShaper();

        switch (type)
        {
            case MapType.ARCHIPELAGO:
                generator = new ArchipelagoGenerator();
                break;
            case MapType.INLAND_SEA:
                generator = new InlandSeaGenerator();
                break;
            case MapType.HIGHLAND:
                generator = new HighlandGenerator();
                break;
            case MapType.ISLANDS:
                generator = new IslandsGenerator();
                break;
            case MapType.SMALL_CONTINENTS:
                generator = new SmallContinentsGenerator();
                break;
            case MapType.CONTINENTS:
                generator = new ContinentsGenerator();
                break;
            case MapType.CONTINENTS_ISLANDS:
                generator = new ContinentsIslandsGenerator();
                break;
            case MapType.SUPER_CONTINENT:
                generator = new SuperContinentGenerator();
                break;
            case MapType.LAKES:
                generator = new LakesGenerator();
                break;
            case MapType.RANDOM:
                generator = new RandomGenerator();
                break;
        }

        if (generator == null)
        {
            generator = new RandomGenerator();
        }

        // generate landmass and water
        _map.TerrainMap = generator.Generate(size);
        _map.Rows = generator.Rows;
        _map.Columns = generator.Columns;

        // decorate map
        shaper.Generate(_map, temperature, humidity, factorRiver, _riverbed);
    }

    public MapData MapData => _map;

    /// <summary>
    /// print generated map unstructured
    /// </summary>
    /// <returns>a string of the map</returns>
    public string Print()
    {
        string response = string.Empty;
        for(int i = 0; i < _map.Columns; ++i)
        {
            for (int j = 0; j < _map.Rows; ++j)
            {
                response += _map.TerrainMap[j * _map.Columns + i] + " ";
            }
            response += "\n";
        }
        return response;
    }
}
