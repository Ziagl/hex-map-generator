using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;
using HexMapGenerator.enums;
using HexMapGenerator.models;

namespace HexMapGenerator.Tests;

public class UtilsTests
{
    [Fact]
    public void TestConvertMapSize()
    {
        var size = Utils.ConvertMapSize(MapSize.MICRO);
        Assert.Equal(26, size.rows);
        Assert.Equal(44, size.columns);
        size = Utils.ConvertMapSize(MapSize.TINY);
        Assert.Equal(38, size.rows);
        Assert.Equal(60, size.columns);
        size = Utils.ConvertMapSize(MapSize.SMALL);
        Assert.Equal(46, size.rows);
        Assert.Equal(74, size.columns);
        size = Utils.ConvertMapSize(MapSize.MEDIUM);
        Assert.Equal(54, size.rows);
        Assert.Equal(84, size.columns);
        size = Utils.ConvertMapSize(MapSize.LARGE);
        Assert.Equal(60, size.rows);
        Assert.Equal(96, size.columns);
        size = Utils.ConvertMapSize(MapSize.HUGE);
        Assert.Equal(66, size.rows);
        Assert.Equal(106, size.columns);
    }

    [Fact]
    public void TestGetMinMaxValues()
    {
        var data = Utils.GetMinMaxValues<TerrainType>();
        Assert.Equal(1, data.min);
        Assert.Equal(13, data.max);
    }

    [Fact]
    public void TestShuffle()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        Utils.Shuffle(list);
        Assert.Equal(5, list.Count);
        Assert.True(list[0] != 1 || list[1] != 2 || list[2] != 3 || list[3] != 4 || list[4] != 5);
    }

    [Fact]
    public void TestInitializeHexGrid()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        Assert.Equal(9, grid.Count);
        foreach (var tile in grid)
        {
            Assert.Equal(TerrainType.PLAIN, tile.terrain);
        }
    }

    [Fact]
    public void TestNeighborOf()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        var tile = Utils.NeighborOf(grid, 3, 3, new CubeCoordinates(0, 0, 0), Direction.NE);
        Assert.Null(tile);
        tile = Utils.NeighborOf(grid, 3, 3, new CubeCoordinates(0, 0, 0), Direction.E);
        Assert.NotNull(tile);
        Assert.True(new CubeCoordinates(1, 0, -1) == tile!.coordinates);
    }

    [Fact]
    public void TestNeighbors()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        var neighbors = Utils.Neighbors(grid, 3, 3, new CubeCoordinates(0, 0, 0));
        Assert.Equal(2, neighbors.Count);
        neighbors = Utils.Neighbors(grid, 3, 3, new CubeCoordinates(1, 1, -2));
        Assert.Equal(6, neighbors.Count);
    }

}
