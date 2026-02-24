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
    private int seed = 0;
    internal static Random random = new();

    public Generator()
    {
        // empty generator has no additional usage
    }

    public Generator(int seed = 0)
    {
        // initialize random number generator with provided seed
        this.seed = seed;
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
        _map.Seed = seed;
        _map.Type = type;
        _map.Size = size;
        _map.Temperature = temperature;
        _map.Humidity = humidity;
        var mapSize = Utils.ConvertMapSize(size);
        _map.Rows = mapSize.rows;
        _map.Columns = mapSize.columns;

        IMapLandmassGenerator generator = new LandmassGenerator();
        IMapLandscapeShaper shaper = new DefaultShaper();

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
