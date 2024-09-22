using com.hexagonsimulations.Geometry.Hex;
using HexMapGenerator.enums;
using HexMapGenerator.interfaces;
using HexMapGenerator.models;

namespace HexMapGenerator.shapers;

internal class DefaultShaper : IMapLandscapeShaper
{
    private readonly float _factorGrass = 0.3f;
    private readonly float _factorDesert = 0.07f;
    private readonly float _factorReef = 0.05f;
    private readonly float _factorOasis = 0.05f;
    private readonly float _factorSwamp = 0.05f;
    private readonly float _factorWood = 0.3f;

    private Random _random = new();

    public void Generate(MapData map, MapTemperature temperature, MapHumidity humidity, float factorRiver, int riverBed)
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
                    coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = (TerrainType)map.TerrainMap.ElementAt(index),
                    landscape = LandscapeType.NONE,
                    river = RiverType.NONE,
                };
            }
        }

        // how many rivers should we create? depending on map size
        int riverCount = (int)(factorRiver * map.Rows * map.Columns);
        int minRiverLength = 3;

        // generate rivers
        var generatedRivers = this.ComputeRivers(grid, map.Rows, map.Columns, riverCount, minRiverLength, riverBed);
    }

    private List<List<Tile>> ComputeRivers(List<Tile> grid, int rows, int columns, int rivers, int minRiverLength, int riverbed)
    {
        // create a list of mountains
        List<Mountain> mountains = new();
        foreach(var tile in grid)
        {
            if (tile.terrain == TerrainType.MOUNTAIN)
            {
                // for a good river there should be at least 2 tiles between mountain and edge
                if(!Utils.IsTileAtEdge(grid, rows, columns, tile, 2))
                {
                    mountains.Add(new Mountain() { coordinates = tile.coordinates });
                }
            }
        }
        // compute distance to water for each mountain
        foreach(var mountain in mountains)
        {
            var data = Utils.FindNearestTile(grid, rows, columns, mountain.coordinates, Math.Max(rows, columns), TerrainType.SHALLOW_WATER);
            mountain.distanceToWater = data.distance;
        }
        List<List<Tile>> generatedRivers = new();
        int maxTry = 30;
        do
        {
            int mountainIndex = this._random.Next(0, mountains.Count);
            var mountain = mountains[mountainIndex];
            var mountainCoords = mountain.coordinates.ToOffset();
            // check if mountain position is possible
            if (grid[mountainCoords.y * columns + mountainCoords.x].river != RiverType.NONE)
            {
                var riverPath = Utils.CreateRiverPath(grid, rows, columns, mountain, mountain.distanceToRiver + 2);
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
                            int distance = Utils.DistanceToRiver(grid, rows, columns, tile.coordinates, 4);
                            if (distance > 0 && distance <= riverbed)
                            {
                                tile.river = RiverType.RIVERAREA;
                            }
                        }
                    }
                    // generate riverbed
                    Utils.ExpandRiverPath(grid, rows, columns, mountain, riverPath);
                    // add river to list of rivers
                    generatedRivers.Add(riverPath);
                }
            }
            mountains.RemoveAt(mountainIndex);
            --maxTry;
        }while(rivers > generatedRivers.Count && maxTry > 0 && mountains.Count > 0);
        return generatedRivers;
    }
}