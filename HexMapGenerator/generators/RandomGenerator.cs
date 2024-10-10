using HexMapGenerator.Enums;
using HexMapGenerator.Interfaces;
using HexMapGenerator.Models;

namespace HexMapGenerator.Generators;

internal class RandomGenerator : IMapTerrainGenerator
{
    public void Generate(MapData map)
    {
        var terrainTypeRange = Utils.GetMinMaxValues<TerrainType>();

        // create empty grid
        List<int> grid = Enumerable.Repeat(0, map.Rows * map.Columns).ToList();
        
        for (int i = 0; i < grid.Count; ++i)
        {
            grid[i] = Generator.random.Next(terrainTypeRange.min, terrainTypeRange.max + 1);
        }

        map.TerrainMap = grid;
    }
}
