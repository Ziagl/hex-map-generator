using com.hexagonsimulations.Geometry.Hex;
using HexMapGenerator.Enums;
using HexMapGenerator.Interfaces;
using HexMapGenerator.Models;

namespace HexMapGenerator.shapers;

internal class DefaultShaper : IMapLandscapeShaper
{
    private readonly float _factorGrass = 0.5f;
    private readonly float _factorDesert = 0.07f;
    private readonly float _factorReef = 0.05f;
    private readonly float _factorOasis = 0.05f;
    private readonly float _factorSwamp = 0.05f;
    private readonly float _factorWood = 0.3f;

    public void Generate(MapData map, float factorRiver, int riverBed)
    {
        // create empty grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();

        // initialize empty landscape
        for (int column = 0; column < map.Columns; ++column)
        {
            for (int row = 0; row < map.Rows; ++row)
            {
                int index = row * map.Columns + column;
                grid[index] = new Tile
                {
                    Coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = (TerrainType)map.TerrainMap.ElementAt(index),
                    landscape = LandscapeType.NONE,
                    river = RiverType.NONE,
                };
            }
        }

        // how many rivers should we create? depending on map size
        int riverCount = (int)(factorRiver * ((int)map.Size + 1));
        int minRiverLength = 3;

        // generate rivers
        var generatedRivers = this.ComputeRivers(grid, map.Rows, map.Columns, riverCount, minRiverLength, riverBed);

        // generate SNOW tiles and consider temperature
        CreateSnowTiles(grid, map.Rows, map.Temperature);

        // generate TUNDRA and consider temperature
        CreateTundraTiles(grid, map.Rows, map.Temperature);

        // generate GRASS tile and consider humidity
        var affectedTerrain = new List<TerrainType>() { TerrainType.PLAIN, TerrainType.PLAIN_HILLS };
        int defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int grassTiles = (int)(defaultTiles * _factorGrass * (1.5 - 0.5 * (int)map.Humidity));
        var grassDistribution = new TileDistribution(0.0f, 0.5f, 0.1f, 0.4f);
        Utils.AddRandomTerrain(grid, map.Rows, map.Columns, TerrainType.GRASS, TerrainType.GRASS_HILLS, grassTiles, grassDistribution);

        // generate DESERT tile and consider humidity
        int desertTiles = Utils.CountTiles(grid, affectedTerrain);
        desertTiles = (int)(desertTiles * _factorDesert * (0.5 + 0.5 * (int)map.Humidity));
        var desertDistribution = new TileDistribution(0.0f, 0.1f, 0.8f, 0.1f);
        Utils.AddRandomTerrain(grid, map.Rows, map.Columns, TerrainType.DESERT, TerrainType.DESERT_HILLS, desertTiles, desertDistribution);

        // generate REEF tiles
        affectedTerrain = new List<TerrainType>() { TerrainType.DEEP_WATER };
        defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int reefTiles = (int)(defaultTiles * _factorReef);
        var reefDistribution = new TileDistribution(0.0f, 0.4f, 0.2f, 0.4f);
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.REEF, affectedTerrain, reefTiles, reefDistribution);

        if (map.Temperature == MapTemperature.HOT)
        {
            affectedTerrain = new List<TerrainType>() { TerrainType.DESERT };
            defaultTiles = Utils.CountTiles(grid, affectedTerrain);
            int oasisTiles = (int)(defaultTiles * _factorOasis);
            var oasisDistribution = new TileDistribution(0.0f, 0.1f, 0.8f, 0.1f);
            Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.OASIS, affectedTerrain, oasisTiles, oasisDistribution);
        }

        // generate SWAMP tiles
        affectedTerrain = new List<TerrainType>() { TerrainType.GRASS, TerrainType.PLAIN, TerrainType.TUNDRA };
        defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int swampTiles = (int)(defaultTiles * _factorSwamp * (1.5 + 0.5 * (int)map.Humidity));
        var swampDistribution = new TileDistribution(0.2f, 0.5f, 0.0f, 0.3f);
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.SWAMP, affectedTerrain, swampTiles, swampDistribution);

        // generate FOREST tiles
        affectedTerrain = new List<TerrainType>() { TerrainType.GRASS, TerrainType.PLAIN, TerrainType.TUNDRA, TerrainType.GRASS_HILLS, TerrainType.PLAIN_HILLS, TerrainType.TUNDRA_HILLS };
        defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int forestTiles = (int)(defaultTiles * _factorWood * 0.5 * (1.5 - 0.5 * (int)map.Humidity));
        var forestDistribution = new TileDistribution(0.05f, 0.5f, 0.05f, 0.4f);
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.FOREST, affectedTerrain, forestTiles, forestDistribution);

        // generate JUNGLE tiles
        affectedTerrain = new List<TerrainType>() { TerrainType.GRASS, TerrainType.PLAIN, TerrainType.GRASS_HILLS, TerrainType.PLAIN_HILLS };
        defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int jungleTiles = (int)(defaultTiles * _factorWood * 0.5 * (1.5 - 0.5 * (int)map.Humidity));
        var jungleDistribution = new TileDistribution(0.0f, 0.0f, 0.2f, 0.8f);
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.JUNGLE, affectedTerrain, jungleTiles, jungleDistribution);

        // generate VOLCANO tiles
        affectedTerrain = new List<TerrainType>() { TerrainType.MOUNTAIN };
        defaultTiles = Utils.CountTiles(grid, affectedTerrain);
        int volcanoTiles = Generator.random.Next(0, Math.Min(10, (int)(defaultTiles / 10)));
        var volcanoDistribution = new TileDistribution();
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.VOLCANO, affectedTerrain, volcanoTiles, volcanoDistribution);

        // add landscape and river to map
        List<int> landscapeTiles = new();
        List<int> riverTiles = new();
        foreach (var tile in grid)
        {
            landscapeTiles.Add((int)tile.landscape);
            riverTiles.Add((int)tile.river);
        }
        map.LandscapeMap = landscapeTiles;
        map.RiverMap = riverTiles;
        map.TerrainMap = Utils.ConvertGrid(grid);

        // generate river directions
        foreach (var river in generatedRivers)
        {
            map.RiverTileDirections = map.RiverTileDirections.Concat(Utils.GenerateRiverTileDirections(river))
                                                             .ToDictionary(dict => dict.Key, dict => dict.Value);
        }
    }

    // compute given number of rivers on given grid and returns them, riverbed defines range around
    // river no other river can be added to map
    private List<List<Tile>> ComputeRivers(List<Tile> grid, int rows, int columns, int rivers, int minRiverLength, int riverbed)
    {
        //early exit
        if(rivers == 0)
        {
            return new();
        }
        // create a list of mountains
        List<Mountain> mountains = new();
        foreach(var tile in grid)
        {
            if (tile.terrain == TerrainType.MOUNTAIN)
            {
                // for a good river there should be at least 2 tiles between mountain and edge
                if(!Utils.IsTileAtEdge(grid, rows, columns, tile, 2))
                {
                    mountains.Add(new Mountain() { Coordinates = tile.Coordinates });
                }
            }
        }
        // compute distance to water for each mountain
        foreach(var mountain in mountains)
        {
            var data = Utils.FindNearestTile(grid, rows, columns, mountain.Coordinates, Math.Max(rows, columns), TerrainType.SHALLOW_WATER);
            mountain.distanceToWater = data.distance;
        }
        List<List<Tile>> generatedRivers = new();
        int maxTry = 30;
        do
        {
            int mountainIndex = Generator.random.Next(0, mountains.Count);
            var mountain = mountains[mountainIndex];
            var mountainCoords = mountain.Coordinates.ToOffset();
            // check if mountain position is possible
            if (grid[mountainCoords.y * columns + mountainCoords.x].river == RiverType.NONE)
            {
                var riverPath = Utils.CreateRiverPath(grid, rows, columns, mountain, mountain.distanceToWater + 2);
                // forbid river if it is too short
                if(riverPath.Count >= minRiverLength)
                {
                    // mark all river tiles on the grid
                    foreach(var tile in riverPath)
                    {
                        tile.river = RiverType.RIVER;
                    }
                    // mark all close to river tiles as riverbed
                    foreach (var tile in grid)
                    {
                        if (tile.river == RiverType.NONE)
                        {
                            // compute distance to river
                            int distance = Utils.DistanceToRiver(grid, rows, columns, tile.Coordinates, 4);
                            if (distance > 0 && distance <= riverbed)
                            {
                                tile.river = RiverType.RIVERAREA;
                            }
                        }
                    }
                    // generate riverbed
                    Utils.ExtendRiverPath(grid, rows, columns, mountain, riverPath);
                    // add river to list of rivers
                    generatedRivers.Add(riverPath);
                }
            }
            mountains.RemoveAt(mountainIndex);
            --maxTry;
        }while(rivers > generatedRivers.Count && maxTry > 0 && mountains.Count > 0);
        return generatedRivers;
    }

    // create snow tiles in polar region
    private void CreateSnowTiles(List<Tile> grid, int rows, MapTemperature temperature)
    {
        foreach (var tile in grid)
        {
            int chance = 0;
            if (tile.Coordinates.r == 0 || tile.Coordinates.r == rows - 1)
            {
                chance = 10;
            }
            if ((int)temperature < (int)MapTemperature.HOT)
            {
                if (tile.Coordinates.r == 1 || tile.Coordinates.r == rows - 2)
                {
                    if ((int)temperature < (int)MapTemperature.NORMAL)
                    {
                        chance = 6;
                    }
                    else
                    {
                        chance = 4;
                    }
                }
            }
            if ((int)temperature < (int)MapTemperature.NORMAL)
            {
                if (tile.Coordinates.r == 2 || tile.Coordinates.r == rows - 3)
                {
                    chance = 4;
                }
            }

            if (chance > 0 && Generator.random.Next(0, 10) < chance)
            {
                switch (tile.terrain)
                {
                    case TerrainType.SHALLOW_WATER:
                    case TerrainType.DEEP_WATER:
                        tile.landscape = LandscapeType.ICE;
                        break;
                    case TerrainType.PLAIN_HILLS:
                        tile.terrain = TerrainType.SNOW_HILLS;
                        break;
                    case TerrainType.PLAIN:
                        tile.terrain = TerrainType.SNOW;
                        break;
                }
            }
        }
    }

    // create snow tiles in polar region
    private void CreateTundraTiles(List<Tile> grid, int rows, MapTemperature temperature)
    {
        foreach (var tile in grid)
        {
            int chance = 0;
            if (tile.Coordinates.r == 1 || tile.Coordinates.r == rows - 2)
            {
                chance = 10;
            }
            if(tile.Coordinates.r == 2 || tile.Coordinates.r == rows -3)
            {
                switch(temperature)
                {
                    case MapTemperature.HOT:
                        chance = 3;
                        break;
                    case MapTemperature.NORMAL:
                        chance = 8;
                        break;
                    case MapTemperature.COLD:
                        chance = 9;
                        break;
                }
            }
            if ((int)temperature < (int)MapTemperature.NORMAL)
            {
                if (tile.Coordinates.r == 3 || tile.Coordinates.r == rows - 4)
                {
                    chance = 6;
                }
                if (tile.Coordinates.r == 4 || tile.Coordinates.r == rows - 5)
                {
                    chance = 3;
                }
            }

            if (chance > 0 && Generator.random.Next(0, 10) < chance)
            {
                switch (tile.terrain)
                {
                    case TerrainType.PLAIN_HILLS:
                        tile.terrain = TerrainType.TUNDRA_HILLS;
                        break;
                    case TerrainType.PLAIN:
                        tile.terrain = TerrainType.TUNDRA;
                        break;
                }
            }
        }
    }
}