using com.hexagonsimulations.HexMapBase.Enums;
using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Models;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class UtilsTests
{
    private List<int> exampleMapEasy = new() {
        2, 2, 2, 2, 2, 2, 2, 2,
        2, 3, 4, 4, 3, 3, 3, 2,
        2, 3, 4, 13, 4, 3, 3, 2,
        2, 3, 4, 4, 3, 3, 3, 2,
        2, 3, 3, 3, 3, 3, 3, 2,
        2, 3, 3, 3, 3, 3, 3, 2,
        2, 3, 3, 3, 3, 3, 3, 2,
        2, 2, 2, 2, 2, 2, 2, 2,
    };
    private CubeCoordinates mountainCoordinateEasy = new CubeCoordinates(2, 2, -4);

    [TestMethod]
    public void TestConvertMapSize()
    {
        var size = Utils.ConvertMapSize(MapSize.MICRO);
        Assert.AreEqual(26, size.rows);
        Assert.AreEqual(44, size.columns);
        size = Utils.ConvertMapSize(MapSize.TINY);
        Assert.AreEqual(38, size.rows);
        Assert.AreEqual(60, size.columns);
        size = Utils.ConvertMapSize(MapSize.SMALL);
        Assert.AreEqual(46, size.rows);
        Assert.AreEqual(74, size.columns);
        size = Utils.ConvertMapSize(MapSize.MEDIUM);
        Assert.AreEqual(54, size.rows);
        Assert.AreEqual(84, size.columns);
        size = Utils.ConvertMapSize(MapSize.LARGE);
        Assert.AreEqual(60, size.rows);
        Assert.AreEqual(96, size.columns);
        size = Utils.ConvertMapSize(MapSize.HUGE);
        Assert.AreEqual(66, size.rows);
        Assert.AreEqual(106, size.columns);
    }

    [TestMethod]
    public void TestGetMinMaxValues()
    {
        var data = Utils.GetMinMaxValues<TerrainType>();
        Assert.AreEqual(1, data.min);
        Assert.AreEqual(13, data.max);
    }

    [TestMethod]
    public void TestShuffle()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        Utils.Shuffle(list);
        Assert.HasCount(5, list);
        Assert.IsTrue(list[0] != 1 || list[1] != 2 || list[2] != 3 || list[3] != 4 || list[4] != 5);
    }

    [TestMethod]
    public void TestIsTileAtEdge()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        Assert.IsTrue(Utils.IsTileAtEdge(grid, 3, 3, grid[0], 1));
        Assert.IsFalse(Utils.IsTileAtEdge(grid, 3, 3, grid[4], 1));
    }

    [TestMethod]
    public void TestFindNearestTile()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        grid[0].terrain = TerrainType.SHALLOW_WATER;
        var data = Utils.FindNearestTile(grid, 3, 3, new CubeCoordinates(0, 2, -2), 3, TerrainType.SHALLOW_WATER);
        Assert.IsGreaterThan(0, data.distance);
        Assert.IsNotNull(data.destinationTile);
        Assert.IsTrue(data.destinationTile.Coordinates == new CubeCoordinates(0, 0, 0));
    }

    [TestMethod]
    public void TestInitializeHexGrid()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        Assert.HasCount(9, grid);
        foreach (var tile in grid)
        {
            Assert.AreEqual(TerrainType.PLAIN, tile.terrain);
        }
    }

    [TestMethod]
    public void TestNeighborOf()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        var tile = Utils.NeighborOf(grid, 3, 3, new CubeCoordinates(0, 0, 0), Direction.NE);
        Assert.IsNull(tile);
        tile = Utils.NeighborOf(grid, 3, 3, new CubeCoordinates(0, 0, 0), Direction.E);
        Assert.IsNotNull(tile);
        Assert.IsTrue(new CubeCoordinates(1, 0, -1) == tile!.Coordinates);
    }

    [TestMethod]
    public void TestNeighbors()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        var baseTile = new Tile() { Coordinates = new CubeCoordinates(0, 0, 0) };
        var neighbors = baseTile.Neighbors(grid.Cast<HexTile>().ToList(), 3, 3);
        Assert.HasCount(2, neighbors);
        baseTile = new Tile() { Coordinates = new CubeCoordinates(1, 1, -2) };
        neighbors = baseTile.Neighbors(grid.Cast<HexTile>().ToList(), 3, 3);
        Assert.HasCount(6, neighbors);
    }

    [TestMethod]
    public void TestCreateRiverPath()
    {
        int mapSize = 8;
        var grid = Enumerable.Repeat(new Tile(), mapSize * mapSize).ToList();
        Utils.InitializeExampleHexGrid(grid, mapSize, mapSize, exampleMapEasy);
        var mountain = new Mountain() { Coordinates = mountainCoordinateEasy };
        int distanceToWater = Utils.FindNearestTile(grid, mapSize, mapSize, mountain.Coordinates, mapSize, TerrainType.SHALLOW_WATER).distance;
        Assert.IsGreaterThan(0, distanceToWater);
        var path = Utils.CreateRiverPath(grid, mapSize, mapSize, mountain, distanceToWater);
        Assert.IsGreaterThan(0, path.Count);
    }

    [TestMethod]
    public void TestFindCommonTile()
    {
        List<Tile> array1 = new() { new Tile() { Coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { Coordinates = new CubeCoordinates(1, 0,-1) },
                                    new Tile() { Coordinates = new CubeCoordinates(0, 1,-1) } };
        List<Tile> array2 = new() { new Tile() { Coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { Coordinates = new CubeCoordinates(1, 1,-2) },
                                    new Tile() { Coordinates = new CubeCoordinates(1, 0,-1) },
                                    new Tile() { Coordinates = new CubeCoordinates(2, 0,-2) }};
        List<Tile> array3 = new() { new Tile() { Coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { Coordinates = new CubeCoordinates(0, 1,-1) },
                                    new Tile() { Coordinates = new CubeCoordinates(-1, 2,-1) },
                                    new Tile() { Coordinates = new CubeCoordinates(-1, 3,-2) }};
        var common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array2 });
        Assert.HasCount(2, common);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array3 });
        Assert.HasCount(2, common);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array2, array3 });
        Assert.HasCount(1, common);
    }

    [TestMethod]
    public void TestCountTiles()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.GRASS);
        grid[0].terrain = TerrainType.SHALLOW_WATER;
        grid[^1].terrain = TerrainType.DEEP_WATER;
        Assert.AreEqual(2, Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.SHALLOW_WATER, TerrainType.DEEP_WATER }));
        Assert.AreEqual(7, Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.GRASS }));
    }

    [TestMethod]
    public void MatchPercentage()
    {
        var map = new List<int>() { 1, 1, 1, 1,
                                    2, 1, 1, 3,
                                    1, 2, 2, 3,
                                    1, 1, 1, 1 };
        var types = new List<int>() { 2, 3 };
        double percentage = Utils.MatchPercentage(map, types);
        Assert.AreEqual(0.3125, percentage);
    }

    [TestMethod]
    public void TestCountLandmasses()
    {
        // Test map with 2 water tiles (1) and land tiles forming 2 landmasses (3,4,5)
        // Map layout (4x5):
        // 1 1 1 1 1    (water row)
        // 1 3 3 1 5    (landmass 1: 2 tiles, landmass 2: 1 tile)
        // 1 3 1 1 5    (continues landmass 1 and 2)
        // 1 4 4 1 1    (landmass 1 continues)
        var map = new List<int>() {
            1, 1, 1, 1, 1,
            1, 3, 3, 1, 5,
            1, 3, 1, 1, 5,
            1, 4, 4, 1, 1
        };
        var waterTiles = new List<int>() { 1, 2 };
        
        // Test with threshold = 1 (all landmasses count)
        var (count, landmasses) = Utils.CountLandmasses(map, 5, 4, waterTiles, 1);
        Assert.AreEqual(2, count, "Should find 2 landmasses with threshold 1");
        Assert.HasCount(2, landmasses, "Should return 2 landmasses");
        
        // Find the larger landmass (should be 5 tiles: indices 6,7,11,16,17)
        var largerLandmass = landmasses.OrderByDescending(l => l.Count).First();
        Assert.HasCount(5, largerLandmass, "Larger landmass should have 5 tiles");
        
        // Find the smaller landmass (should be 2 tiles: indices 9,14)
        var smallerLandmass = landmasses.OrderBy(l => l.Count).First();
        Assert.HasCount(2, smallerLandmass, "Smaller landmass should have 2 tiles");
        
        // Test with threshold = 3 (only landmass with 5 tiles counts)
        var (count2, landmasses2) = Utils.CountLandmasses(map, 5, 4, waterTiles, 3);
        Assert.AreEqual(1, count2, "Should find 1 landmass with threshold 3");
        Assert.HasCount(1, landmasses2, "Should return 1 landmass meeting threshold");
        Assert.HasCount(5, landmasses2[0], "Returned landmass should have 5 tiles");
        
        // Test with threshold = 10 (no landmasses meet threshold)
        var (count3, landmasses3) = Utils.CountLandmasses(map, 5, 4, waterTiles, 10);
        Assert.AreEqual(0, count3, "Should find 0 landmasses with threshold 10");
        Assert.HasCount(0, landmasses3, "Should return empty list");
    }

    [TestMethod]
    public void TestCountLandmassesSingleLandmass()
    {
        // All land tiles forming one connected landmass
        var map = new List<int>() {
            3, 3, 3,
            3, 1, 3,
            3, 3, 3
        };
        var waterTiles = new List<int>() { 1 };
        
        var (count, landmasses) = Utils.CountLandmasses(map, 3, 3, waterTiles, 1);
        Assert.AreEqual(1, count, "Should find 1 connected landmass");
        Assert.HasCount(8, landmasses[0], "Landmass should have 8 tiles (all except center)");
    }

    [TestMethod]
    public void TestCountLandmassesAllWater()
    {
        // All water - no landmasses
        var map = new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2 };
        var waterTiles = new List<int>() { 1, 2 };
        
        var (count, landmasses) = Utils.CountLandmasses(map, 4, 2, waterTiles, 1);
        Assert.AreEqual(0, count, "Should find 0 landmasses");
        Assert.HasCount(0, landmasses, "Should return empty list");
    }
}