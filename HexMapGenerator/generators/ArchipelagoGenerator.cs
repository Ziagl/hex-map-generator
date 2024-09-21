using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.generators;

internal class ArchipelagoGenerator : IMapTerrainGenerator
{
    private readonly MapType _type = MapType.ARCHIPELAGO;
    private MapSize _size = MapSize.TINY;
    private int _rows = 0;
    private int _columns = 0;

    private readonly float _factorLand = 0.8f;
    private readonly float _factorWater = 0.5f;
    private readonly float _factorContinentalDrift = 0.2f;
    private readonly float _factorMountain = 0.03f;
    private readonly float _factorHills = 0.06f;

    public List<int> Generate(MapSize size)
    {
        this._size = size;
        var mapSize = Utils.ConvertMapSize(this._size);
        this._rows = mapSize.rows;
        this._columns = mapSize.columns;
        var random = new Random();

        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), this._rows * this._columns).ToList();

        // 1. create a plain map
        Utils.InitializeHexGrid(grid, this._rows, this._columns, TerrainType.SHALLOW_WATER);

        // 2. add randomly continents
        int landTiles = (int)(grid.Count * this._factorLand);
        int islandCounter = random.Next(20, 40); // number of islands
        // set island seeds to the map with numbering MAXCONTINENTSEED - continentCounter
        Utils.AddRandomContinentSeed(grid, this._rows, this._columns, TerrainType.SHALLOW_WATER, islandCounter);

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
                    var neighbors = Utils.RandomNeighbors(grid, this._rows, this._columns, tile.coordinates);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.continentSeed == 0 && landTiles > 0)
                        {
                            // check if an adjacent field is not from another continent (continents should not touch!)
                            var tileNeighbors = Utils.Neighbors(grid, this._rows, this._columns, neighbor.coordinates);
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
        Utils.ExpandContinentDrift(grid, this._rows, this._columns, waterTiles);
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
        Utils.AddRandomTileSeed(grid, this._rows, this._columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeCounter, waterTiles);
        // expand lakes
        Utils.ExpandWater(grid, this._rows, this._columns, lakeTiles, waterTiles);
        // create deep water tiles
        Utils.ShallowToDeepWater(grid, this._rows, this._columns);

        // 5. create random hills
        int mountainTiles = (int)(grid.Count * this._factorMountain);
        int hillTiles = (int)(grid.Count * this._factorHills);
        hillTiles = hillTiles + mountainTiles; // mountains can only be generated from hills
        int hillCounter = (int)(hillTiles / random.Next(5, 8)); // number of mountain ranges
        List<Tile> mountainRangesTiles = new();
        Utils.AddRandomTileSeed(grid, this._rows, this._columns, mountainRangesTiles, TerrainType.PLAIN_HILLS, TerrainType.PLAIN, hillCounter, hillTiles);

        // 6. expand hills
        Utils.ExpandHills(grid, this._rows, this._columns, mountainRangesTiles, hillTiles);

        // 7. create mountain tiles
        Utils.HillsToMountains(grid, this._rows, this._columns, mountainTiles);

        return Utils.ConvertGrid(grid);
    }

    public int Rows => this._rows;

    public int Columns => this._columns;
}
