using com.hexagonsimulations.HexMapBase.Enums;
using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapGenerator.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapGenerator.Models;

public record MapData
{
    // Tile index maps (row-major, size Rows * Columns)
    public List<int> TerrainMap { get; set; } = new();
    public List<int> LandscapeMap { get; set; } = new();
    public List<int> RiverMap { get; set; } = new();

    // River outgoing edge directions per cube coordinate
    public Dictionary<CubeCoordinates, List<Direction>> RiverTileDirections { get; set; } = new();

    public int Rows { get; set; }
    public int Columns { get; set; }
    public int Seed { get; set; }

    public MapType Type { get; set; } = MapType.RANDOM;
    public MapSize Size { get; set; } = MapSize.MEDIUM;
    public MapTemperature Temperature { get; set; } = MapTemperature.NORMAL;
    public MapHumidity Humidity { get; set; } = MapHumidity.NORMAL;

    // JSON options for serialization
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter() // Enums as readable strings
        }
    };

    /// <summary>
    /// Serialize this MapData instance to a JSON string.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }

    /// <summary>
    /// Deserialize a JSON string into a MapData instance.
    /// </summary>
    public static MapData FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

        return JsonSerializer.Deserialize<MapData>(json, JsonOptions)
               ?? throw new JsonException("Failed to deserialize MapData from JSON.");
    }

    /// <summary>
    /// Write this MapData instance to a binary writer.
    /// </summary>
    public void Write(BinaryWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.Write(Rows);
        writer.Write(Columns);
        writer.Write(Seed);
        writer.Write((int)Type);
        writer.Write((int)Size);
        writer.Write((int)Temperature);
        writer.Write((int)Humidity);

        // Write TerrainMap
        writer.Write(TerrainMap.Count);
        foreach (var value in TerrainMap)
        {
            writer.Write(value);
        }

        // Write LandscapeMap
        writer.Write(LandscapeMap.Count);
        foreach (var value in LandscapeMap)
        {
            writer.Write(value);
        }

        // Write RiverMap
        writer.Write(RiverMap.Count);
        foreach (var value in RiverMap)
        {
            writer.Write(value);
        }

        // Write RiverTileDirections
        writer.Write(RiverTileDirections.Count);
        foreach (var kvp in RiverTileDirections)
        {
            kvp.Key.Write(writer); // Assuming CubeCoordinates has a Write method
            writer.Write(kvp.Value.Count);
            foreach (var direction in kvp.Value)
            {
                writer.Write((int)direction);
            }
        }
    }

    /// <summary>
    /// Read a MapData instance from a binary reader.
    /// </summary>
    public static MapData Read(BinaryReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var map = new MapData
        {
            Rows = reader.ReadInt32(),
            Columns = reader.ReadInt32(),
            Seed = reader.ReadInt32(),
            Type = (MapType)reader.ReadInt32(),
            Size = (MapSize)reader.ReadInt32(),
            Temperature = (MapTemperature)reader.ReadInt32(),
            Humidity = (MapHumidity)reader.ReadInt32()
        };

        // Read TerrainMap
        var terrainCount = reader.ReadInt32();
        map.TerrainMap = new List<int>(terrainCount);
        for (int i = 0; i < terrainCount; i++)
        {
            map.TerrainMap.Add(reader.ReadInt32());
        }

        // Read LandscapeMap
        var landscapeCount = reader.ReadInt32();
        map.LandscapeMap = new List<int>(landscapeCount);
        for (int i = 0; i < landscapeCount; i++)
        {
            map.LandscapeMap.Add(reader.ReadInt32());
        }

        // Read RiverMap
        var riverCount = reader.ReadInt32();
        map.RiverMap = new List<int>(riverCount);
        for (int i = 0; i < riverCount; i++)
        {
            map.RiverMap.Add(reader.ReadInt32());
        }

        // Read RiverTileDirections
        var riverTileCount = reader.ReadInt32();
        map.RiverTileDirections = new Dictionary<CubeCoordinates, List<Direction>>(riverTileCount);
        for (int i = 0; i < riverTileCount; i++)
        {
            var key = CubeCoordinates.Read(reader); // Assuming CubeCoordinates has a Read method
            var directionCount = reader.ReadInt32();
            var directions = new List<Direction>(directionCount);
            for (int j = 0; j < directionCount; j++)
            {
                directions.Add((Direction)reader.ReadInt32());
            }
            map.RiverTileDirections[key] = directions;
        }

        return map;
    }
}
