using com.hexagonsimulations.HexMapGenerator.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.hexagonsimulations.HexMapGenerator.Serialization;

/// <summary>
/// Central JSON (de)serialization helpers for MapData.
/// </summary>
public static class MapDataJson
{
    // Reuse a single configured options instance
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter() // Enums as readable strings
        }
    };

    public static string Serialize(MapData? map)
    {
        if (map is null)
        {
            return string.Empty; // Explicitly return "null" for null input
        }

        return JsonSerializer.Serialize(map, Options);
    }

    public static MapData? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null; // Return null for empty or whitespace JSON
        }

        try
        {
            return JsonSerializer.Deserialize<MapData>(json, Options);
        }
        catch (JsonException)
        {
            return null; // Return null for invalid JSON
        }
    }
}
