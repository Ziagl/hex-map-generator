using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.generators;

internal class RandomGenerator : IMapTerrainGenerator
{
    public void Generate(MapData map)
    {
        var random = new Random();
        var terrainTypeRange = Utils.GetMinMaxValues<TerrainType>();

        // create empty grid
        List<int> grid = Enumerable.Repeat(0, map.Rows * map.Columns).ToList();
        
        for (int i = 0; i < grid.Count; ++i)
        {
            grid[i] = random.Next(terrainTypeRange.min, terrainTypeRange.max + 1);
        }

        map.TerrainMap = grid;
    }
}
