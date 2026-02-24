using com.hexagonsimulations.HexMapGenerator;
using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;
using com.hexagonsimulations.HexMapHeightmap;

namespace HexMapGenerator.Generators.Heightmap;

internal class SuperContinentGenerator : IMapHeightmapGenerator
{
    private readonly double _factorLand = 0.85;
    //private readonly float _factorMountain = 0.05f;
    //private readonly float _factorHills = 0.08f;

    public void GenerateHeightmap(MapData map)
    {
        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.DEEP_WATER);

        // 2. create super continent landmass
        var waterTiles = new List<int>() { (int)TerrainType.DEEP_WATER, (int)TerrainType.SHALLOW_WATER };
        double _waterLevel = 1.0;
        double waterPercentage = 1.0;
        int landmassCount = 0;
        List<List<int>> validLandmasses = new();
        var heightmapGenerator = new HeightmapGenerator(map.Seed);
        var heightmap = heightmapGenerator.BlendHeightmaps(
            heightmapGenerator.GeneratePerlinNoise(map.Columns, map.Rows, 30.0, 6, 0.5, 2.0),
            heightmapGenerator.GenerateEllipticContinent(map.Columns, map.Rows, _factorLand),
            0.75);
        do
        {
            // apply heightmap to grid
            for (int row = 0; row < map.Rows; ++row)
            {
                for (int col = 0; col < map.Columns; ++col)
                {
                    int index = row * map.Columns + col;
                    double height = heightmap[col, row];

                    if (height <= _waterLevel)
                    {
                        grid[index] = new Tile { terrain = TerrainType.DEEP_WATER };
                    }
                    else
                    {
                        grid[index] = new Tile { terrain = TerrainType.PLAIN };
                    }
                }
            }
            map.TerrainMap = Utils.ConvertGrid(grid);

            // compute map statistics
            waterPercentage = Utils.MatchPercentage(map.TerrainMap, waterTiles);
            (landmassCount, validLandmasses) = Utils.CountLandmasses(map.TerrainMap, map.Columns, map.Rows, waterTiles, 25);

            // change water level
            if(waterPercentage > 1.0 - _factorLand)
            {
                _waterLevel -= 0.02;
            }
        } while (landmassCount != 1 || waterPercentage > 1.0 - _factorLand);

        // update map terrain
        map.TerrainMap = Utils.ConvertGrid(grid);
    }

    /*public void Generate(MapData map)
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
    }*/
}
