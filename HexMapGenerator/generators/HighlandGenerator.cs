using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.generators;

internal class HighlandGenerator : IMapTerrainGenerator
{
    private readonly MapType _type = MapType.HIGHLAND;
    private MapSize _size = MapSize.TINY;
    private int _rows = 0;
    private int _columns = 0;

    private readonly float _factorWater = 0.15f;
    private readonly float _factorMountain = 0.1f;
    private readonly float _factorHills = 0.2f;

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
        Utils.InitializeHexGrid(grid, this._rows, this._columns, TerrainType.PLAIN);

        // 2. add randomly lakes
        int waterTiles = (int)(grid.Count * this._factorWater);
        int lakeCounter = waterTiles / random.Next(5, 8); // number of lakes (fifth, sixth or seventh of max number of tiles)
        List<Tile> lakeTiles = new();
        Utils.AddRandomTileSeed(grid, this._rows, this._columns, lakeTiles, TerrainType.SHALLOW_WATER, TerrainType.PLAIN, lakeCounter, waterTiles);

        // 3. expand lakes
        Utils.ExpandWater(grid, this._rows, this._columns, lakeTiles, waterTiles);

        // 4. create deep water tiles
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
