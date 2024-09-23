using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;
using HexMapGenerator.enums;
using HexMapGenerator.models;

namespace HexMapGenerator.Tests;

public class UtilsTests
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
    public void TestIsTileAtEdge()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        Assert.True(Utils.IsTileAtEdge(grid, 3, 3, grid[0], 1));
        Assert.False(Utils.IsTileAtEdge(grid, 3, 3, grid[4], 1));
    }

    [Fact]
    public void TestFindNearestTile()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.PLAIN);
        grid[0].terrain = TerrainType.SHALLOW_WATER;
        var data = Utils.FindNearestTile(grid, 3, 3, new CubeCoordinates(0, 2, -2), 3, TerrainType.SHALLOW_WATER);
        Assert.True(data.distance > 0);
        Assert.NotNull(data.destinationTile);
        Assert.True(data.destinationTile.coordinates == new CubeCoordinates(0, 0, 0));
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

    [Fact]
    public void TestCreateRiverPath()
    {
        int mapSize = 8;
        var grid = Enumerable.Repeat(new Tile(), mapSize * mapSize).ToList();
        Utils.InitializeExampleHexGrid(grid, mapSize, mapSize, exampleMapEasy);
        var mountain = new Mountain() { coordinates = mountainCoordinateEasy };
        int distanceToWater = Utils.FindNearestTile(grid, mapSize, mapSize, mountain.coordinates, mapSize, TerrainType.SHALLOW_WATER).distance;
        Assert.True(distanceToWater > 0);
        var path = Utils.CreateRiverPath(grid, mapSize, mapSize, mountain, distanceToWater);
        Assert.True(path.Count > 0);
    }

    [Fact]
    public void TestFindComminTile()
    {
        List<Tile> array1 = new() { new Tile() { coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { coordinates = new CubeCoordinates(1, 0,-1) },
                                    new Tile() { coordinates = new CubeCoordinates(0, 1,-1) },
                                    new Tile() { coordinates = new CubeCoordinates(1, 1,-2) } };
        List<Tile> array2 = new() { new Tile() { coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { coordinates = new CubeCoordinates(1, 0,-1) },
                                    new Tile() { coordinates = new CubeCoordinates(2, 0,-2) }};
        List<Tile> array3 = new() { new Tile() { coordinates = new CubeCoordinates(0, 0, 0) },
                                    new Tile() { coordinates = new CubeCoordinates(0, 1,-1) },
                                    new Tile() { coordinates = new CubeCoordinates(-1, 2,-1) },
                                    new Tile() { coordinates = new CubeCoordinates(-1, 3,-2) }};
        var common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array2 });
        Assert.Equal(2, common.Count);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array3 });
        Assert.Equal(2, common.Count);
        common = Utils.FindCommonTiles(new List<List<Tile>> { array1, array2, array3 });
        Assert.Single(common);
    }
    
    [Fact]
    public void TestCountTiles()
    {
        var grid = Enumerable.Repeat(new Tile(), 9).ToList();
        Utils.InitializeHexGrid(grid, 3, 3, TerrainType.GRASS);
        grid[0].terrain = TerrainType.SHALLOW_WATER;
        grid[^1].terrain = TerrainType.DEEP_WATER;
        Assert.True(Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.SHALLOW_WATER, TerrainType.DEEP_WATER }) == 2);
        Assert.True(Utils.CountTiles(grid, new List<TerrainType>() { TerrainType.GRASS }) == 7);
    }
}
