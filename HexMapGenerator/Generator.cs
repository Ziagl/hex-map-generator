using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Generators;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;
using com.hexagonsimulations.HexMapGenerator.shapers;

namespace com.hexagonsimulations.HexMapGenerator;

public class Generator
{
    private readonly int _riverbed = 3;
    private MapData _map = new();
    internal static Random random = new();

    public Generator()
    {
        // empty generator has no additional usage
    }

    public Generator(Int32 seed)
    {
        // initialize random number generator with provided seed
        random = new Random(seed);
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
        _map.Type = type;
        _map.Size = size;
        _map.Temperature = temperature;
        _map.Humidity = humidity;
        var mapSize = Utils.ConvertMapSize(size);
        _map.Rows = mapSize.rows;
        _map.Columns = mapSize.columns;

        IMapTerrainGenerator generator = new RandomGenerator();
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

        // generate landmass and water
        generator.Generate(_map);

        // decorate map
        shaper.Generate(_map, factorRiver, _riverbed);
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
