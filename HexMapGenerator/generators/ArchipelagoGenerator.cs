using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.generators;

internal class ArchipelagoGenerator : IMapTerrainGenerator
{
    private readonly float _factorLand = 0.8f;
    private readonly float _factorWater = 0.5f;
    private readonly float _factorContinentalDrift = 0.2f;
    private readonly float _factorMountain = 0.03f;
    private readonly float _factorHills = 0.06f;

    public void Generate(MapData map)
    {
        var random = new Random();

        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.SHALLOW_WATER);

        // 2. add randomly continents
        int landTiles = (int)(grid.Count * this._factorLand);
        int islandCounter = random.Next(20, 40); // number of islands
        // set island seeds to the map with numbering MAXCONTINENTSEED - continentCounter
        Utils.AddRandomContinentSeed(grid, map.Rows, map.Columns, TerrainType.SHALLOW_WATER, islandCounter);

        // 3. expand continents without touching other continents
        List<(int key, List<Tile> value)> continentTiles = new();
        // create seeds of continents (unique continent id and one tile)
        for (int i = Utils.MAXCONTINENTSEED; i > Utils.MAXCONTINENTSEED - islandCounter; --i)
        {
            foreach(var tile in grid)
            {
                if (tile.continentSeed == i)
                {
                    continentTiles.Add((key: i, value: new List<Tile>() { tile }));
                }
            }
        }
        // fill continent data structures with new tiles
        int loopMax = Utils.MAXLOOPS;
        int minContinentSeed = Utils.MAXCONTINENTSEED - islandCounter + 1;
        do
        {
            int continentToExpand = random.Next(minContinentSeed, Utils.MAXCONTINENTSEED);
            var continentTilesArray = continentTiles.Find(x => x.key == continentToExpand).value;
            if(continentTilesArray is not null)
            {
                Utils.Shuffle(continentTilesArray);
                List<Tile> addedContinentTilesArray = new();
                foreach(var tile in continentTilesArray)
                {
                    var neighbors = Utils.RandomNeighbors(grid, map.Rows, map.Columns, tile.coordinates);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.continentSeed == 0 && landTiles > 0)
                        {
                            // check if an adjacent field is not from another continent (continents should not touch!)
                            var tileNeighbors = Utils.Neighbors(grid, map.Rows, map.Columns, neighbor.coordinates);
                            if(tileNeighbors.Any(tileNeighbor => tileNeighbor.continentSeed == continentToExpand) &&
                               !tileNeighbors.Any(tileNeighbor => tileNeighbor.continentSeed != continentToExpand && tileNeighbor.continentSeed >= minContinentSeed))
                            {
                                neighbor.continentSeed = continentToExpand;
                                --landTiles;
                                addedContinentTilesArray.Add(neighbor);
                            }
                        }
                    }
                }
                continentTilesArray.AddRange(addedContinentTilesArray);
            }
            int index = continentTiles.FindIndex(x => x.key == continentToExpand);
            if(index != -1)
            {
                var data = continentTiles[index];
                if (continentTilesArray is not null)
                {
                    data.value = continentTilesArray;
                }
            }
            --loopMax;
        } while (landTiles > 0 && loopMax > 0);
        // expand random water tiles that are betweed continents
        int waterTiles = (int)(grid.Count * (this._factorWater * this._factorContinentalDrift));
        Utils.ExpandContinentDrift(grid, map.Rows, map.Columns, waterTiles);
        // convert all continent helpers to plains
        foreach (var tile in grid)
        {
            if (tile.continentSeed >= minContinentSeed)
            {
                tile.terrain = TerrainType.PLAIN;
            }
        }

        // 4. add lakes
        waterTiles = (int)(grid.Count * (this._factorWater * (1 - this._factorContinentalDrift)));
        // add randomly lakes
        int lakeCounter = waterTiles / random.Next(5, 8); // number of lakes (fifth, sixth or seventh of max number of tiles)
        List<Tile> lakeTiles = new();
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeCounter, waterTiles);
        // expand lakes
        Utils.ExpandWater(grid, map.Rows, map.Columns, lakeTiles, waterTiles);
        // create deep water tiles
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
