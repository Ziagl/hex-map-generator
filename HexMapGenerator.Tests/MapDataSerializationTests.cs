using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Models;
using System.Text;
using System.Text.Json;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class MapDataSerializationTests
{
    private readonly string TempDir = @"C:\Temp\";
    private readonly bool DumpToDisk = true; // set to true to dump serialized data to disk for inspection

    [TestMethod]
    public void MapData_Json()
    {
        var generator = new Generator(1234);
        generator.GenerateMap(MapType.HIGHLAND, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.NORMAL, 2.0f);
        var original = generator.MapData;

        var json = original.ToJson();
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "JSON should not be empty.");

        if(DumpToDisk)
        {
            File.WriteAllText($"{TempDir}Generator.json", json);
        }

        var roundTripped = MapData.FromJson(json);
        Assert.IsNotNull(roundTripped, "Deserialized MapData should not be null.");

        AssertMapDataEqual(original, roundTripped);
    }

    [TestMethod]
    public void MapData_Binary()
    {
        var generator = new Generator(1234);
        generator.GenerateMap(MapType.HIGHLAND, MapSize.MEDIUM, MapTemperature.NORMAL, MapHumidity.NORMAL, 2.0f);
        var original = generator.MapData;
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            original.Write(bw);
        }

        if (DumpToDisk)
        {
            File.WriteAllBytes($"{TempDir}MapData.bin", ms.ToArray());
        }

        ms.Position = 0;
        MapData fromBinary;
        using (var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: false))
        {
            fromBinary = MapData.Read(br);
        }

        AssertMapDataEqual(original, fromBinary);
    }

    [TestMethod]
    public void SerializeReturnsNonEmptyString()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.LAKES, MapSize.HUGE, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        var json = generator.MapData.ToJson();
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "Serialized JSON should not be empty or whitespace.");
        Assert.IsTrue(json.Contains("{") && json.Contains("}"), "Serialized output does not appear to be JSON.");
    }

    [TestMethod]
    public void SerializeNull()
    {
        MapData? map = null;
        Assert.ThrowsExactly<NullReferenceException>(() => map!.ToJson());
    }

    [TestMethod]
    public void DeserializeInvalidJson()
    {
        var invalidJson = "{ invalid json ";
        Assert.ThrowsExactly<JsonException>(() => MapData.FromJson(invalidJson));
    }

    // Helper to assert deep equality of MapData
    private static void AssertMapDataEqual(MapData expected, MapData actual)
    {
        Assert.AreEqual(expected.Rows, actual.Rows, "Rows mismatch.");
        Assert.AreEqual(expected.Columns, actual.Columns, "Columns mismatch.");
        Assert.AreEqual(expected.Seed, actual.Seed, "Seed mismatch.");
        Assert.AreEqual(expected.Type, actual.Type, "MapType mismatch.");
        Assert.AreEqual(expected.Size, actual.Size, "MapSize mismatch.");
        Assert.AreEqual(expected.Temperature, actual.Temperature, "MapTemperature mismatch.");
        Assert.AreEqual(expected.Humidity, actual.Humidity, "MapHumidity mismatch.");

        // TerrainMap
        CollectionAssert.AreEqual(expected.TerrainMap, actual.TerrainMap, "TerrainMap content mismatch.");

        // LandscapeMap
        CollectionAssert.AreEqual(expected.LandscapeMap, actual.LandscapeMap, "LandscapeMap content mismatch.");

        // RiverMap
        CollectionAssert.AreEqual(expected.RiverMap, actual.RiverMap, "RiverMap content mismatch.");

        // RiverTileDirections
        Assert.HasCount(expected.RiverTileDirections.Count, actual.RiverTileDirections, "RiverTileDirections count mismatch.");
        foreach (var kvp in expected.RiverTileDirections)
        {
            CollectionAssert.AreEqual(kvp.Value, actual.RiverTileDirections[kvp.Key], $"River directions mismatch at {kvp.Key}.");
        }
    }
}
