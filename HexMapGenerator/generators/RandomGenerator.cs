using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;

namespace com.hexagonsimulations.HexMapGenerator.Generators;

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
