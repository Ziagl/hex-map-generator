using com.hexagonsimulations.HexMapBase.Geometry.Hex;
using com.hexagonsimulations.HexMapBase.Geometry.Hex.Enums;
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
        Assert.AreEqual(5, list.Count);
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
        Assert.IsTrue(data.distance > 0);
        Assert.IsNotNull(data.destinationTile);
        Assert.IsTrue(data.destinationTile.Coordinates == new CubeCoordinates(0, 0, 0));
    }

    [TestMethod]
    public void TestInitializeHexGrid()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        Assert.AreEqual(9, grid.Count);
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
        Assert.AreEqual(2, neighbors.Count);
        baseTile = new Tile() { Coordinates = new CubeCoordinates(1, 1, -2) };
        neighbors = baseTile.Neighbors(grid.Cast<HexTile>().ToList(), 3, 3);
        Assert.AreEqual(6, neighbors.Count);
    }

    [TestMethod]
    public void TestCreateRiverPath()
    {
        int mapSize = 8;
        var grid = Enumerable.Repeat(new Tile(), mapSize * mapSize).ToList();
        Utils.InitializeExampleHexGrid(grid, mapSize, mapSize, exampleMapEasy);
        var mountain = new Mountain() { Coordinates = mountainCoordinateEasy };
        int distanceToWater = Utils.FindNearestTile(grid, mapSize, mapSize, mountain.Coordinates, mapSize, TerrainType.SHALLOW_WATER).distance;
        Assert.IsTrue(distanceToWater > 0);
        var path = Utils.CreateRiverPath(grid, mapSize, mapSize, mountain, distanceToWater);
        Assert.IsTrue(path.Count > 0);
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
        Assert.AreEqual(2, common.Count);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array3 });
        Assert.AreEqual(2, common.Count);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array2, array3 });
        Assert.AreEqual(1, common.Count);
    }

    [TestMethod]
    public void TestCountTiles()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.GRASS);
        grid[0].terrain = TerrainType.SHALLOW_WATER;
        grid[^1].terrain = TerrainType.DEEP_WATER;
        Assert.IsTrue(Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.SHALLOW_WATER, TerrainType.DEEP_WATER }) == 2);
        Assert.IsTrue(Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.GRASS }) == 7);
    }
}