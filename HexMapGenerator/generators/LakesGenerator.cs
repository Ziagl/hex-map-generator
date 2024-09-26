using HexMapGenerator.Enums;
using HexMapGenerator.Interfaces;
using HexMapGenerator.Models;

namespace HexMapGenerator.Generators;

internal class LakesGenerator : IMapTerrainGenerator
{
    private readonly float _factorWater = 0.2f;
    private readonly float _factorMountain = 0.06f;
    private readonly float _factorHills = 0.07f;

    public void Generate(MapData map)
    {
        var random = new Random();

        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.PLAIN);

        // 2. add randomly lakes
        int waterTiles = (int)(grid.Count * this._factorWater);
        int lakeCounter = waterTiles / random.Next(15, 30); // number of lakes (fifth, sixth or seventh of max number of tiles)
        List<Tile> lakeTiles = new();
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeCounter, waterTiles);

        // 3. expand lakes
        Utils.ExpandWater(grid, map.Rows, map.Columns, lakeTiles, waterTiles);

        // 4. create deep water tiles
        Utils.ShallowToDeepWater(grid, map.Rows, map.Columns);

        // 5. create random hills
        int mountainTiles = (int)(grid.Count * this._factorMountain);
        int hillTiles = (int)(grid.Count * this._factorHills);
        hillTiles = hillTiles + mountainTiles; // mountains can only be generated from hills
        int hillCounter = (int)(hillTiles / random.Next(5, 8)); // number of mountain ranges
        List<Tile> mountainRangesTiles = new();
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, mountainRangesTiles, TerrainType.PLAIN_HILLS, TerrainType.PLAIN, hillCounter, hillTiles);

        // 6. expand hills
        Utils.ExpandHills(grid, map.Rows, map.Columns, mountainRangesTiles, hillTiles);

        // 7. create mountain tiles
        Utils.HillsToMountains(grid, map.Rows, map.Columns, mountainTiles);

        map.TerrainMap = Utils.ConvertGrid(grid);
    }
}