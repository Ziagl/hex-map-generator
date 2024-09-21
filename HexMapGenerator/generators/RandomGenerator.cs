using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.generators;

internal class RandomGenerator : IMapTerrainGenerator
{
    private readonly MapType _type = MapType.RANDOM;
    private MapSize _size = MapSize.TINY;
    private int _rows = 0;
    private int _columns = 0;

    public List<int> Generate(MapSize size)
    {
        this._size = size;
        var terrainTypeRange = Utils.GetMinMaxValues<TerrainType>();
        var mapSize = Utils.ConvertMapSize(this._size);
        this._rows = mapSize.rows;
        this._columns = mapSize.columns;

        // create empty grid
        List<int> grid = Enumerable.Repeat(0, this._rows * this._columns).ToList();

        var random = new Random();
        for (int i = 0; i < grid.Count; ++i)
        {
            grid[i] = random.Next(terrainTypeRange.min, terrainTypeRange.max + 1);
        }

        return grid;
    }

    public int Rows => this._rows;
    public int Columns => this._columns;
}
