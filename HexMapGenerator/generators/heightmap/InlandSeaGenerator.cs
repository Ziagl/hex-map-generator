using com.hexagonsimulations.HexMapGenerator;
using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;

namespace HexMapGenerator.Generators.Heightmap;

internal class InlandSeaGenerator : IMapHeightmapGenerator
{
    private readonly float _factorWater = 0.3f;
    private readonly float _factorMountain = 0.1f;
    private readonly float _factorHills = 0.1f;

    public void GenerateHeightmap(MapData map)
    {

    }

    /*public void Generate(MapData map)
    {
        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.PLAIN);

        // 2. add a lake in the middle of map
        int waterTiles = (int)(grid.Count * this._factorWater);
        int tileFactor = waterTiles / 5;
        int lakeCounter = Generator.random.Next(tileFactor / 2, tileFactor + 1); // number of lakes (fifth, sixth or seventh of max number of tiles)
        List<Tile> lakeTiles = new();
        for(int i = 0; i < lakeCounter; ++i)
        {
            Tile? tile = null;
            int rowBorder = map.Rows / 5;
            int columnBorder = map.Columns / 5;
            int loopMax = Utils.MAXLOOPS;
            do
            {
                tile = Utils.RandomTile(grid, map.Rows, map.Columns);
                if(tile is not null)
                {
                    var coords = tile.Coordinates.ToOffset();
                    if(coords.y >= rowBorder &&
                       coords.y < map.Rows - rowBorder &&
                       coords.x >= columnBorder &&
                       coords.x < map.Columns - columnBorder)
                    {
                        tile.terrain = TerrainType.SHALLOW_WATER;
                        --waterTiles;
                        lakeTiles.Add(tile);
                        break;
                    }
                }
                --loopMax;
            } while (loopMax > 0 && waterTiles > 0);
        }
        int randomSeeds = Generator.random.Next(5, 16);
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, randomSeeds, waterTiles);

        // 3. expand lakes
        Utils.ExpandWater(grid, map.Rows, map.Columns, lakeTiles, waterTiles);

        // 4. create deep water tiles
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
    }*/
}
