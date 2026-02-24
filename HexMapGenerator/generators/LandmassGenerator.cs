using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Interfaces;
using com.hexagonsimulations.HexMapGenerator.Models;
using HexMapGenerator.Generators.Heightmap;

namespace com.hexagonsimulations.HexMapGenerator.Generators;

internal class LandmassGenerator : IMapLandmassGenerator
{
    public void Generate(MapData map)
    {
        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();
        // 1. create a plain map
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.DEEP_WATER);
        // 2. create heightmap depending on type
        IMapHeightmapGenerator generator = new RandomGenerator();
        switch(map.Type)
        {
            /*case MapType.ARCHIPELAGO:
                generator = new ArchipelagoGenerator();
                break;
            case MapType.INLAND_SEA:
                generator = new InlandSeaGenerator();
                break;
            case MapType.HIGHLAND:
                generator = new HighlandGenerator();
                break;
            case MapType.ISLANDS:
                generator = new IslandsGenerator();
                break;
            case MapType.SMALL_CONTINENTS:
                generator = new SmallContinentsGenerator();
                break;
            case MapType.CONTINENTS:
                generator = new ContinentsGenerator();
                break;
            case MapType.CONTINENTS_ISLANDS:
                generator = new ContinentsIslandsGenerator();
                break;*/
            case MapType.SUPER_CONTINENT:
                generator = new SuperContinentGenerator();
                break;
            /*case MapType.LAKES:
                generator = new LakesGenerator();
                break;
            case MapType.RANDOM:
                generator = new RandomGenerator();
                break;*/
        }
        generator.GenerateHeightmap(map);
    }
}
