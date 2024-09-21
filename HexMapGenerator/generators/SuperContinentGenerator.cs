using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexMapGenerator.generators;

internal class SuperContinentGenerator : IMapTerrainGenerator
{
    private readonly MapType _type = MapType.SUPER_CONTINENT;
    private MapSize _size = MapSize.TINY;
    private int _rows = 0;
    private int _columns = 0;

    private readonly float _factorLand = 0.7f;
    private readonly float _factorMountain = 0.05f;
    private readonly float _factorHills = 0.08f;

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

        // 2. create super continent landmass
        int landTiles = (int)(grid.Count * this._factorLand);
        int tileFactor = landTiles / 6;
        int landCounter = random.Next(tileFactor / 2, tileFactor); // number of landmasses (sixth or seventh of max number of tiles)
        List<Tile> plainTiles = new();
        for (int i = 0; i < landCounter; ++i)
        {
            Tile? tile = null;
            int rowBorder = this._rows / 5;
            int columnBorder = this._columns / 5;
            int loopMax = Utils.MAXLOOPS;
            do
            {
                tile = Utils.RandomTile(grid, this._rows, this._columns);
                if (tile is not null)
                {
                    var coords = tile.coordinates.ToOffset();
                    if (coords.y >= rowBorder &&
                       coords.y < this._rows - rowBorder &&
                       coords.x >= columnBorder &&
                       coords.x < this._columns - columnBorder)
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
        int randomSeeds = random.Next(5, 15);
        Utils.AddRandomTileSeed(grid, this._rows, this._columns, plainTiles, TerrainType.PLAIN, TerrainType.SHALLOW_WATER, randomSeeds, landTiles);

        // 3. expand land
        Utils.ExpandLand(grid, this._rows, this._columns, plainTiles, landTiles);

        // 4. add lakes
        int lakeSeeds = random.Next(10, 21);
        int waterTiles = lakeSeeds * random.Next(4, 8);
        List<Tile> lakeTiles = new();
        Utils.AddRandomTileSeed(grid, this._rows, this._columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeSeeds, waterTiles);
        // expand lakes
        Utils.ExpandWater(grid, this._rows, this._columns, lakeTiles, waterTiles);
        // create deep water tiles
        Utils.ShallowToDeepWater(grid, this._rows, this._columns);

        // 5. create random hills
        int mountainTiles = (int)(grid.Count * this._factorMountain);
        int hillTiles = (int)(grid.Count * this._factorHills);
        hillTiles = hillTiles + mountainTiles; // mountains can only be generated from hills
        int hillCounter = (int)(hillTiles / random.Next(8, 13)); // number of mountain ranges
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
