using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;
using HexMapGenerator.enums;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HexMapGenerator.Tests")]

namespace HexMapGenerator.models;

internal class Utils
{
    public static readonly int MAXLOOPS = 10000;
    public static readonly int MAXCONTINENTSEED = 999;

    internal static (int rows, int columns) ConvertMapSize(MapSize size)
    {
        int rows = 26;
        int columns = 44;

        switch (size)
        {
            case MapSize.MICRO: // 1144 tiles
                rows = 26;
                columns = 44;
                break;
            case MapSize.TINY: // 2280 tiles
                rows = 38;
                columns = 60;
                break;
            case MapSize.SMALL: // 3404 tiles
                rows = 46;
                columns = 74;
                break;
            case MapSize.MEDIUM: // 4536 tiles
                rows = 54;
                columns = 84;
                break;
            case MapSize.LARGE: // 5760 tiles
                rows = 60;
                columns = 96;
                break;
            case MapSize.HUGE: // 6996 tiles
                rows = 66;
                columns = 106;
                break;
        }

        return (rows, columns);
    }

    internal static (int min, int max) GetMinMaxValues<T>()
        where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<int>();
        int minValue = values.Min();
        int maxValue = values.Max();
        return (min: minValue, max: maxValue);
    }

    public static void Shuffle<T>(List<T> list)
    {
        Random random = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<int> ConvertGrid(List<Tile> grid)
    {
        List<int> tiles = new();
        foreach (var tile in grid)
        {
            tiles.Add((int)tile.terrain);
        }
        return tiles;
    }

    // initialize a grid and each tile with given type and coordinates
    internal static void InitializeHexGrid(List<Tile> grid, int rows, int columns, TerrainType type)
    {
        for (int column = 0; column < columns; ++column)
        {
            for (int row = 0; row < rows; ++row)
            {
                grid[row * columns + column] = new Tile
                {
                    coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = type,
                };
            }
        }
    }

    // returns a random tile of given grid
    internal static Tile RandomTile(List<Tile> grid, int rows, int columns)
    {
        var random = new Random();
        int row = random.Next(0, rows);
        int column = random.Next(0, columns);
        return grid[row * columns + column];
    }

    // add random tiles of given type
    internal static void AddRandomTileSeed(
        List<Tile> grid,
        int rows,
        int columns,
        List<Tile> tiles,
        TerrainType type,
        TerrainType oldType,
        int count,
        int maxCount
    )
    {
        for (int i = 0; i < count; ++i)
        {
            int loopMax = Utils.MAXLOOPS;
            Tile? tile = null;
            do
            {
                tile = Utils.RandomTile(grid, rows, columns);
                --loopMax;
            } while (loopMax > 0 && tile.terrain != oldType);
            if (tile != null)
            {
                tile.terrain = type;
                --maxCount;
                tiles.Add(tile);
            }
        }
    }

    // add random continents of given type
    internal static void AddRandomContinentSeed(
        List<Tile> grid,
        int rows,
        int columns,
        TerrainType oldType,
        int count
    )
    {
        for (int i = 0; i < count; ++i)
        {
            int loopMax = Utils.MAXLOOPS;
            Tile? tile = null;
            do
            {
                tile = Utils.RandomTile(grid, rows, columns);
                --loopMax;
            } while (loopMax > 0 && tile.terrain != oldType);
            if (tile != null)
            {
                tile.continentSeed = Utils.MAXCONTINENTSEED - i;
            }
        }
    }

    // get neighbor tile or null if it is out of bounds
    public static Tile? NeighborOf(
        List<Tile> grid,
        int rows,
        int columns,
        CubeCoordinates coordinates,
        Direction direction
    )
    {
        var neighborCoordinates = coordinates.Neighbor(direction);

        // bounding checks
        var coord = neighborCoordinates.ToOffset();
        if (coord.x < 0 || coord.x >= columns || coord.y < 0 || coord.y >= rows)
        {
            return null;
        }

        // get neighbor
        return grid[coord.y * columns + coord.x];
    }

    // get all neighbors of given grid and coordinate
    internal static List<Tile> Neighbors(List<Tile> grid, int rows, int columns, CubeCoordinates coordinates)
    {
        List<Tile> neighbors = new();

        foreach (var neighborCoordinates in coordinates.Neighbors())
        {
            var coord = neighborCoordinates.ToOffset();
            if (coord.x < 0 || coord.x >= columns || coord.y < 0 || coord.y >= rows)
            {
                continue;
            }
            neighbors.Add(grid[coord.y * columns + coord.x]);
        }

        return neighbors;
    }

    // returns a random neighbor of given grid and coordinate
    internal static List<Tile> RandomNeighbors(List<Tile> grid, int rows, int columns, CubeCoordinates coordinates)
    {
        List<Tile> neighbors = new();
        var allNeighbors = Utils.Neighbors(grid, rows, columns, coordinates);
        var random = new Random();

        // randomly select neighbors
        foreach(var neighbor in allNeighbors)
        {
            if (random.Next(0, 2) == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    internal static void ExpandContinentDrift(List<Tile> grid, int rows, int columns, int waterTiles)
    {
        int loopMax = Utils.MAXLOOPS;
        do
        {
            // get random tile
            var tile = Utils.RandomTile(grid, rows, columns);
            if(tile.continentSeed == 0)
            {
                continue;
            }

            var neighbors = Utils.Neighbors(grid, rows, columns, tile.coordinates);
            int continentCounter = 0;
            foreach(var neighbor in neighbors)
            {
                if (neighbor.continentSeed > 0)
                {
                    ++continentCounter;
                }
            }
            if(continentCounter >= 3)
            {
                tile.continentSeed = 0;
                --waterTiles;
            }

            --loopMax;
        } while (waterTiles > 0 && loopMax > 0);
    }

    // expand given landTiles randomly till number of landTiles were placed
    internal static void ExpandLand(List<Tile> grid, int rows, int columns, List<Tile> plainTiles, int landTiles)
    {
        int loopMax = Utils.MAXLOOPS;
        do
        {
            Utils.Shuffle(plainTiles);
            List<Tile> additionalPlainTiles = new();
            foreach (var tile in plainTiles)
            {
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile.coordinates);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.terrain != TerrainType.PLAIN &&
                        landTiles > 0)
                    {
                        neighbor.terrain = TerrainType.PLAIN;
                        --landTiles;
                        additionalPlainTiles.Add(neighbor);
                    }
                }
            }
            plainTiles.AddRange(additionalPlainTiles);

            --loopMax;
        } while (loopMax > 0 && landTiles > 0);
    }

    // expand given lakeTiles randomly till number of waterTiles were placed
    internal static void ExpandWater(List<Tile> grid, int rows, int columns, List<Tile> lakeTiles, int waterTiles)
    {
        int loopMax = Utils.MAXLOOPS;
        do
        {
            Utils.Shuffle(lakeTiles);
            List<Tile> addedLakeTiles = new();
            lakeTiles.ForEach(tile =>
            {
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile.coordinates);
                neighbors.ForEach(neighbor =>
                {
                    if (neighbor.terrain != TerrainType.SHALLOW_WATER &&
                        neighbor.terrain != TerrainType.DEEP_WATER &&
                        waterTiles > 0)
                    {
                        neighbor.terrain = TerrainType.SHALLOW_WATER;
                        --waterTiles;
                        addedLakeTiles.Add(neighbor);
                    }
                });
            });
            lakeTiles.AddRange(addedLakeTiles);
            --loopMax;
        } while (waterTiles > 0 && loopMax > 0);
    }

    // expand given hillTiles randomly till number of hillTiles were placed
    internal static void ExpandHills(List<Tile> grid, int rows, int columns, List<Tile> mountainRangesTiles, int hillTiles)
    {
        int loopMax = Utils.MAXLOOPS;
        do
        {
            Utils.Shuffle(mountainRangesTiles);
            List<Tile> addedMountainRangesTiles = new();
            mountainRangesTiles.ForEach(tile =>
            {
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile.coordinates);
                neighbors.ForEach(neighbor =>
                {
                    if (neighbor.terrain != TerrainType.SHALLOW_WATER &&
                        neighbor.terrain != TerrainType.DEEP_WATER &&
                        neighbor.terrain != TerrainType.PLAIN_HILLS &&
                        hillTiles > 0)
                    {
                        neighbor.terrain = TerrainType.PLAIN_HILLS;
                        --hillTiles;
                        addedMountainRangesTiles.Add(neighbor);
                    }
                });
            });
            mountainRangesTiles.AddRange(addedMountainRangesTiles);
            --loopMax;
        } while (hillTiles > 0 && loopMax > 0);
    }

    // turn hills into mountains till number of mountains is reached
    internal static void HillsToMountains(List<Tile> grid, int rows, int columns, int mountainTiles)
    {
        int loopMax = Utils.MAXLOOPS;
        do
        {
            var tile = Utils.RandomTile(grid, rows, columns);
            if (tile != null && tile.terrain == TerrainType.PLAIN_HILLS)
            {
                var neighbors = Utils.Neighbors(grid, rows, columns, tile.coordinates);
                // if all neighbors are hill tiles or water -> tile is mountain tile
                if (neighbors.All(neighbor => neighbor.terrain == TerrainType.PLAIN_HILLS ||
                                              neighbor.terrain == TerrainType.MOUNTAIN ||
                                              neighbor.terrain == TerrainType.SHALLOW_WATER ||
                                              neighbor.terrain == TerrainType.DEEP_WATER) &&
                    !neighbors.All(neighbor => neighbor.terrain == TerrainType.SHALLOW_WATER ||
                                               neighbor.terrain == TerrainType.DEEP_WATER))
                {
                    tile.terrain = TerrainType.MOUNTAIN;
                    --mountainTiles;
                }else
                {
                    // if there is maximum one other additional tile
                    int countOthers = 0;
                    foreach (var neighbor in neighbors)
                    {
                        if(neighbor.terrain != TerrainType.MOUNTAIN &&
                           neighbor.terrain != TerrainType.PLAIN_HILLS &&
                           neighbor.terrain != TerrainType.SHALLOW_WATER &&
                           neighbor.terrain != TerrainType.DEEP_WATER)
                        {
                            ++countOthers;
                        }
                    }

                    if (countOthers <= 1)
                    {
                        tile.terrain = TerrainType.MOUNTAIN;
                        --mountainTiles;
                    }
                }
            }
            
            --loopMax;
        } while (mountainTiles > 0 && loopMax > 0);
    }

    // turns all shallow water tiles into deep water tiles if they are fully surrounded by water
    internal static void ShallowToDeepWater(List<Tile> grid, int rows, int columns)
    {
        grid.ForEach(tile =>
        {
            if (tile.terrain == TerrainType.SHALLOW_WATER)
            {
                var neighbors = Utils.Neighbors(grid, rows, columns, tile.coordinates);
                if (neighbors.All(neighbor => neighbor.terrain == TerrainType.DEEP_WATER || neighbor.terrain == TerrainType.SHALLOW_WATER))
                {
                    tile.terrain = TerrainType.DEEP_WATER;
                }
            }
        });
    }
}
