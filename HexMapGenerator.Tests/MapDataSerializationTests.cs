using com.hexagonsimulations.HexMapBase.Enums;
using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Models;
using com.hexagonsimulations.HexMapGenerator.Serialization;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class MapDataSerializationTests
{
    [TestMethod]
    public void SerializeReturnsNonEmptyString()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.LAKES, MapSize.HUGE, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        var json = MapDataJson.Serialize(generator.MapData);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "Serialized JSON should not be empty or whitespace.");
        Assert.IsTrue(json.Contains("{") && json.Contains("}"), "Serialized output does not appear to be JSON.");
    }

    [TestMethod]
    public void SerializationDeserialization()
    {
        var generator = new Generator();
        generator.GenerateMap(MapType.LAKES, MapSize.HUGE, MapTemperature.COLD, MapHumidity.WET, 0.0f);
        var original = generator.MapData;

        var json = MapDataJson.Serialize(original);
        Assert.IsFalse(string.IsNullOrWhiteSpace(json), "JSON should not be empty.");

        var roundTripped = MapDataJson.Deserialize(json);
        Assert.IsNotNull(roundTripped, "Deserialized MapData should not be null.");

        AssertMapDataEqual(original, roundTripped);
    }

    [TestMethod]
    public void SerializeNull()
    {
        var json = MapDataJson.Serialize(null!);
        Assert.AreEqual(string.Empty, json.Trim(), "Serializing null should return an empty string.");
    }

    [TestMethod]
    public void DeserializeInvalidJson()
    {
        var invalidJson = "{ invalid json ";
        var result = MapDataJson.Deserialize(invalidJson);
        Assert.IsNull(result, "Deserializing invalid JSON should return null.");
    }

    // Helper to assert deep equality of MapData
    private static void AssertMapDataEqual(MapData expected, MapData actual)
    {
        Assert.AreEqual(expected.Rows, actual.Rows, "Rows mismatch.");
        Assert.AreEqual(expected.Columns, actual.Columns, "Columns mismatch.");
        Assert.AreEqual(expected.Type, actual.Type, "MapType mismatch.");
        Assert.AreEqual(expected.Size, actual.Size, "MapSize mismatch.");
        Assert.AreEqual(expected.Temperature, actual.Temperature, "MapTemperature mismatch.");
        Assert.AreEqual(expected.Humidity, actual.Humidity, "MapHumidity mismatch.");

        // TerrainMap
        AssertIsNotNullAndSameCount(expected.TerrainMap, actual.TerrainMap, "TerrainMap null or count mismatch.");
        CollectionAssert.AreEqual(expected.TerrainMap.ToList(), actual.TerrainMap.ToList(), "TerrainMap content mismatch.");

        // LandscapeMap
        AssertIsNotNullAndSameCount(expected.LandscapeMap, actual.LandscapeMap, "LandscapeMap null or count mismatch.");
        CollectionAssert.AreEqual(expected.LandscapeMap.ToList(), actual.LandscapeMap.ToList(), "LandscapeMap content mismatch.");

        // RiverMap
        AssertIsNotNullAndSameCount(expected.RiverMap, actual.RiverMap, "RiverMap null or count mismatch.");
        CollectionAssert.AreEqual(expected.RiverMap.ToList(), actual.RiverMap.ToList(), "RiverMap content mismatch.");

        // RiverTileDirections
        if (expected.RiverTileDirections is null && actual.RiverTileDirections is null)
            return;

        Assert.IsNotNull(expected.RiverTileDirections, "Expected RiverTileDirections is null while actual isn't.");
        Assert.IsNotNull(actual.RiverTileDirections, "Actual RiverTileDirections is null while expected isn't.");
        Assert.AreEqual(expected.RiverTileDirections.Count, actual.RiverTileDirections.Count, "RiverTileDirections count mismatch.");

        foreach (var kvp in expected.RiverTileDirections)
        {
            Assert.IsTrue(actual.RiverTileDirections.ContainsKey(kvp.Key), $"Missing RiverTile key {kvp.Key}.");
            var expectedDirs = kvp.Value ?? new List<Direction>();
            var actualDirs = actual.RiverTileDirections[kvp.Key] ?? new List<Direction>();
            CollectionAssert.AreEqual(expectedDirs.ToList(), actualDirs.ToList(), $"River directions mismatch at {kvp.Key}.");
        }
    }

    private static void AssertIsNotNullAndSameCount<T>(ICollection<T>? expected, ICollection<T>? actual, string message)
    {
        Assert.IsNotNull(expected, $"Expected collection null: {message}");
        Assert.IsNotNull(actual, $"Actual collection null: {message}");
        Assert.AreEqual(expected.Count, actual.Count, $"Count mismatch: {message}");
    }
}
