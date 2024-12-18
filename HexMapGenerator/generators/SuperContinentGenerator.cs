using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;

namespace com.hexagonsimulations.HexMapGenerator.Generators;

internal class SuperContinentGenerator : IMapTerrainGenerator
{
    private readonly float _factorLand = 0.7f;
    private readonly float _factorMountain = 0.05f;
    private readonly float _factorHills = 0.08f;

    public void Generate(MapData map)
    {
        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.SHALLOW_WATER);

        // 2. create super continent landmass
        int landTiles = (int)(grid.Count * this._factorLand);
        int tileFactor = landTiles / 6;
        int landCounter = Generator.random.Next(tileFactor / 2, tileFactor); // number of landmasses (sixth or seventh of max number of tiles)
        List<Tile> plainTiles = new();
        for (int i = 0; i < landCounter; ++i)
        {
            Tile? tile = null;
            int rowBorder = map.Rows / 5;
            int columnBorder = map.Columns / 5;
            int loopMax = Utils.MAXLOOPS;
            do
            {
                tile = Utils.RandomTile(grid, map.Rows, map.Columns);
                if (tile is not null)
                {
                    var coords = tile.Coordinates.ToOffset();
                    if (coords.y >= rowBorder &&
                       coords.y < map.Rows - rowBorder &&
                       coords.x >= columnBorder &&
                       coords.x < map.Columns - columnBorder)
                    {
                        tile.terrain = TerrainType.PLAIN;
                        --landTiles;
                        plainTiles.Add(tile);
                        break;
                    }
                }
                --loopMax;
            } while (loopMax > 0 && landTiles > 0);
        }
        int randomSeeds = Generator.random.Next(5, 15);
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, plainTiles, TerrainType.PLAIN, TerrainType.SHALLOW_WATER, randomSeeds, landTiles);

        // 3. expand land
        Utils.ExpandLand(grid, map.Rows, map.Columns, plainTiles, landTiles);

        // 4. add lakes
        int lakeSeeds = Generator.random.Next(10, 21);
        int waterTiles = lakeSeeds * Generator.random.Next(4, 8);
        List<Tile> lakeTiles = new();
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeSeeds, waterTiles);
        // expand lakes
        Utils.ExpandWater(grid, map.Rows, map.Columns, lakeTiles, waterTiles);
        // create deep water tiles
        Utils.ShallowToDeepWater(grid, map.Rows, map.Columns);

        // 5. create random hills
        int mountainTiles = (int)(grid.Count * this._factorMountain);
        int hillTiles = (int)(grid.Count * this._factorHills);
        hillTiles = hillTiles + mountainTiles; // mountains can only be generated from hills
        int hillCounter = (int)(hillTiles / Generator.random.Next(8, 13)); // number of mountain ranges
        List<Tile> mountainRangesTiles = new();
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, mountainRangesTiles, TerrainType.PLAIN_HILLS, TerrainType.PLAIN, hillCounter, hillTiles);

        // 6. expand hills
        Utils.ExpandHills(grid, map.Rows, map.Columns, mountainRangesTiles, hillTiles);

        // 7. create mountain tiles
        Utils.HillsToMountains(grid, map.Rows, map.Columns, mountainTiles);

        map.TerrainMap = Utils.ConvertGrid(grid);
    }
}
