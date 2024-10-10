using com.hexagonsimulations.Geometry.Hex;
using com.hexagonsimulations.Geometry.Hex.Enums;
using HexMapGenerator.Enums;
using System.Data;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HexMapGenerator.Tests")]

namespace HexMapGenerator.Models;

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
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Generator.random.Next(n + 1);
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

    // checks if tile is at least distance tiles away from edge of grid
    public static bool IsTileAtEdge(List<Tile> grid, int rows, int columns, Tile tile, int distance)
    {
        // first compute expected number of elements for given distance
        int totalElements = 1;
        for(int i = 1; i <= distance; ++i)
        {
            totalElements += i * 6;
        }
        // compute spiral and compare with total elements
        var spiral = tile.Coordinates.SpiralAroundInward(distance, Direction.W);
        int counter = 0;
        foreach(var coordinate in spiral)
        {
            var coord = coordinate.ToOffset();
            if (coord.x >= 0 && coord.x < columns && coord.y >= 0 && coord.y < rows)
            {
                ++counter;
            }
        }
        return counter != totalElements;
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
                    Coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = type,
                };
            }
        }
    }

    // initialize a grid and each tile with given type and coordinates
    internal static void InitializeExampleHexGrid(List<Tile> grid, int rows, int columns, List<int> map)
    {
        for (int column = 0; column < columns; ++column)
        {
            for (int row = 0; row < rows; ++row)
            {
                int index = row * columns + column;
                grid[index] = new Tile
                {
                    Coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = (TerrainType)map[index],
                };
            }
        }
    }

    // returns a random tile of given grid
    internal static Tile RandomTile(List<Tile> grid, int rows, int columns)
    {
        int row = Generator.random.Next(0, rows);
        int column = Generator.random.Next(0, columns);
        return grid[row * columns + column];
    }

    // finds nearest tile of given type or undefined if it not found
    internal static (Tile? destinationTile, int distance) FindNearestTile(List<Tile> grid, int rows, int columns, CubeCoordinates coordinates, int maxRadius, TerrainType type)
    {
        int distance = 0;
        int radius = 1;
        Tile? destinationTile = null;
        do
        {
            var tileCoordinates = coordinates.RingAround(radius, Direction.W);
            foreach (var tileCoordinate in tileCoordinates)
            {
                var coord = tileCoordinate.ToOffset();
                if (coord.x >= 0 && coord.x < columns && coord.y >= 0 && coord.y < rows)
                {
                    var tile = grid[coord.y * columns + coord.x];
                    if (tile.terrain == type)
                    {
                        distance = radius;
                        destinationTile = tile;
                        break;
                    }
                }
            }
            ++radius;
        } while (radius <= maxRadius && distance == 0);
        return (destinationTile, distance);
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

    // returns a random neighbor of given grid and coordinate
    internal static List<Tile> RandomNeighbors(List<Tile> grid, int rows, int columns, Tile baseTile)
    {
        List<Tile> neighbors = new();
        var allNeighbors = baseTile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();

        // randomly select neighbors
        foreach(var neighbor in allNeighbors)
        {
            if (Generator.random.Next(0, 2) == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // counts given types in given grid
    internal static int CountTiles(List<Tile> grid, List<TerrainType> types)
    {
        int count = 0;
        for (int i = 0; i < grid.Count; i++)
        {
            if (types.Contains(grid[i].terrain))
            {
                ++count;
            }
        }
        return count;
    }

    // extends water border between two continents
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

            var neighbors = tile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();
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
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile);
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
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile);
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
                var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile);
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
                var neighbors = tile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();
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
                var neighbors = tile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();
                if (neighbors.All(neighbor => neighbor.terrain == TerrainType.DEEP_WATER || neighbor.terrain == TerrainType.SHALLOW_WATER))
                {
                    tile.terrain = TerrainType.DEEP_WATER;
                }
            }
        });
    }

    // returns an array of map rows for each climate zone
    internal static List<List<int>> ClimateZonesSeparation(int rows)
    {
        List<List<int>> climateZoneRows = new();
        int lastRows = 0;
        foreach(var zoneSize in TileDistribution.climateZoneSizes)
        {
            // create new array of lines for climate zone
            List<int> rowNumbers = new();
            // rows per hemisphere
            int neededRows = (int)Math.Round(zoneSize * rows / 2);
            for(int i = 0; i < neededRows; ++i)
            {
                rowNumbers.Add(lastRows + i);
                rowNumbers.Add(rows - (lastRows + i + 1));
            }
            climateZoneRows.Add(rowNumbers);
            lastRows += rowNumbers.Count / 2;
        }
        return climateZoneRows;
    }

    // returns a random tile of given grid
    internal static Tile RandomTileOfRow(List<Tile> grid, int rows, int columns, int row)
    {
        if(row < 0 || row > rows)
        {
            row = 0;
        }
        int column = Generator.random.Next(0, columns);
        return grid[row * columns + column];
    }

    // converts given number of plain terrain tiles to given tile type
    internal static void AddRandomTerrain(List<Tile> grid, int rows, int columns, TerrainType typeFlat, TerrainType typeHill, int count, TileDistribution distribution)
    {
        int loopMax = Utils.MAXLOOPS;
        var rowsPerZone = Utils.ClimateZonesSeparation(rows);
        List<int> tilesPerZone = new();
        tilesPerZone.Add((int)(distribution.polar * count));
        tilesPerZone.Add((int)(distribution.temperate * count));
        tilesPerZone.Add((int)(distribution.dry * count));
        tilesPerZone.Add((int)(distribution.tropical * count));
        int currentZone = 0;
        do
        {
            // place tile for current zone
            if (tilesPerZone[currentZone] > 0)
            {
                int randomRowIndex = Generator.random.Next(0, rowsPerZone[currentZone].Count);
                var tile = Utils.RandomTileOfRow(grid, rows, columns, rowsPerZone[currentZone][randomRowIndex]);
                if (tile.terrain == TerrainType.PLAIN)
                {
                    tile.terrain = typeFlat;
                    --tilesPerZone[currentZone];
                    --count;
                }
                else if (tile.terrain == TerrainType.PLAIN_HILLS)
                {
                    tile.terrain = typeHill;
                    --tilesPerZone[currentZone];
                    --count;
                }
            }
            else
            {
                if (currentZone < tilesPerZone.Count - 1)
                {
                    ++currentZone;
                }
                else
                {
                    // place random tile instead? TODO
                    count = 0;
                }
            }
            --loopMax;
        } while (count > 0 && loopMax > 0);
    }

    // adds given landscape type to given terrain tiles
    internal static void AddRandomLandscape(List<Tile> grid, int rows, int columns, LandscapeType type, List<TerrainType> terrains, int count, TileDistribution distribution)
    {
        int loopMax = Utils.MAXLOOPS;
        var rowsPerZone = Utils.ClimateZonesSeparation(rows);
        List<int> tilesPerZone = new();
        tilesPerZone.Add((int)(distribution.polar * count));
        tilesPerZone.Add((int)(distribution.temperate * count));
        tilesPerZone.Add((int)(distribution.dry * count));
        tilesPerZone.Add((int)(distribution.tropical * count));
        int currentZone = 0;
        do
        {
            // place tile for current zone
            if (tilesPerZone[currentZone] > 0)
            {
                int randomRowIndex = Generator.random.Next(0, rowsPerZone[currentZone].Count);
                var tile = Utils.RandomTileOfRow(grid, rows, columns, rowsPerZone[currentZone][randomRowIndex]);
                if(terrains.Contains(tile.terrain))
                {
                    tile.landscape = type;
                    --tilesPerZone[currentZone];
                    --count;
                }
            }
            else
            {
                if(currentZone < tilesPerZone.Count - 1)
                {
                    ++currentZone;
                }
                else
                {
                    // place random landscape instead? TODO
                    count = 0;
                }
            }
            --loopMax;
        } while (count > 0 && loopMax > 0);
    }

    // creates a path from given mountain to a water tile nearby
    internal static List<Tile> CreateRiverPath(List<Tile> grid, int rows, int columns, Mountain mountain, int maxLength)
    {
        List<Tile> riverPath = new();
        var mountainCoords = mountain.Coordinates.ToOffset();
        var mountainTile = grid[mountainCoords.y * columns + mountainCoords.x];
        List<Tile> openList = new();
        List<Tile> closedList = new();
        int loopMax = Utils.MAXLOOPS;
        var nextTile = mountainTile;
        bool success = false;
        int lastDistance = 0;
        do
        {
            // add current open list to closed list
            if(openList.Count > 0)
            {
                closedList.AddRange(openList);
                openList.Clear();
            }
            // get neighbors and add neighbors to open list
            var neighbors = nextTile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();
            foreach (var neighbor in neighbors)
            {
                if (success == false)
                {
                    if (neighbor.terrain == TerrainType.SHALLOW_WATER ||
                       neighbor.terrain == TerrainType.DEEP_WATER)
                    {
                        // END found water tile -> clear open list
                        openList.Clear();
                        success = true;
                    }
                    else if (neighbor.terrain != TerrainType.MOUNTAIN &&
                            neighbor.river == RiverType.NONE)
                    {
                        // if it is not a mountain tile and not already in closed list
                        if (!closedList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
            // select next tile
            if (openList.Count > 0 && success == false)
            {
                List<Tile> possibleTiles = new();
                // in near surrounding of mountain, make sure next tile is further away from mountain as last tile
                if (lastDistance < 2)
                {
                    for (int i = 0; i < openList.Count; ++i)
                    {
                        var distanceToMountain = mountainTile.Coordinates.DistanceTo(openList[i].Coordinates);
                        if (distanceToMountain > lastDistance)
                        {
                            possibleTiles.Add(openList[i]);
                        }
                    }
                }
                // after some tiles away river should start to find water
                else
                {
                    possibleTiles = openList;
                }
                if (possibleTiles.Count > 0)
                {
                    // determine next tile
                    if (Generator.random.Next(0, 4) == 0)
                    {
                        // option 1: random tile
                        nextTile = possibleTiles[Generator.random.Next(0, possibleTiles.Count)];
                        lastDistance = mountainTile.Coordinates.DistanceTo(nextTile.Coordinates);
                    }
                    else
                    {
                        // option 2: sort by distanceToWater first
                        List<(Tile tile, int distanceToWater)> sortedTiles = new();
                        foreach (var tile in possibleTiles)
                        {
                            var data = Utils.FindNearestTile(grid, rows, columns, tile.Coordinates, 20, TerrainType.SHALLOW_WATER);
                            if (data.destinationTile is not null)
                            {
                                sortedTiles.Add((tile, data.distance));
                            }
                        }
                        sortedTiles.Sort((a, b) => a.distanceToWater - b.distanceToWater);
                        nextTile = sortedTiles.First().tile;
                    }
                    lastDistance = mountainTile.Coordinates.DistanceTo(nextTile.Coordinates);
                    riverPath.Add(nextTile);
                }
                else
                {
                    // END no possible tiles for river
                    closedList.AddRange(openList);
                    openList.Clear();
                }
            }
            // if river is too long, stop computing it
            if(riverPath.Count > maxLength)
            {
                // END max length exceeded
                closedList.AddRange(openList);
                openList.Clear();
            }
            --loopMax;
        } while (loopMax > 0 && openList.Count > 0);
        
        return riverPath;
    }

    // computes the distance to next river tile
    internal static int DistanceToRiver(List<Tile> grid, int rows, int columns, CubeCoordinates coordinates, int maxDistance)
    {
        int distance = 0;
        int radius = 1;
        do
        {
            var tileCoordinates = coordinates.RingAround(radius, Direction.W);
            foreach (var tileCoordinate in tileCoordinates)
            {
                var coord = tileCoordinate.ToOffset();
                if (coord.x >= 0 && coord.x < columns && coord.y >= 0 && coord.y < rows)
                {
                    var tile = grid[coord.y * columns + coord.x];
                    if (tile.river == RiverType.RIVER)
                    {
                        distance = radius;
                        break;
                    }
                }
            }
            ++radius;
        } while (distance == 0 && radius <= maxDistance);
        return distance;
    }

    // returns all elements that are in every given array
    internal static List<Tile> FindCommonTiles(List<List<Tile>> arrays)
    {
        // early exit
        if (arrays == null || arrays.Count == 0)
        {
            return new List<Tile>();
        }
       
        // start with the first list
        var commonTiles = new HashSet<Tile>(arrays[0]);

        // intersect with each subsequent list
        foreach (var array in arrays.Skip(1))
        {
            commonTiles.IntersectWith(array);
        }

        return commonTiles.ToList();
    }

    internal static void ExtendRiverPath(List<Tile> grid, int rows, int columns, Mountain mountain, List<Tile> riverPath)
    {
        // so there is now a path of single tiles, append it for a second tile
        List<List<Tile>> riverTileNeighbors = new();
        var mountainTileNeighbors = mountain.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList();
        foreach(var tile in riverPath)
        {
            riverTileNeighbors.Add(tile.Neighbors(grid.Cast<HexTile>().ToList(), rows, columns).Cast<Tile>().ToList());
        }
        // special case river path of 1 tile
        if (riverPath.Count == 1)
        {
            throw new Exception("River path of 1 tile is not supported");
        }
        // start by finding first river bank tile
        List<Tile> otherRiverBank = new();
        var sharedTiles = Utils.FindCommonTiles(new List<List<Tile>>() { mountainTileNeighbors, riverTileNeighbors[0], riverTileNeighbors[1] });
        if(sharedTiles.Count == 1)
        {
            otherRiverBank.Add(sharedTiles[0]);
        }
        else
        {
            var localSharedTiles = Utils.FindCommonTiles(new List<List<Tile>>() { mountainTileNeighbors, riverTileNeighbors[0] });
            if(localSharedTiles.Count != 2)
            {
                throw new Exception("Error: special case for first tile of river failed.");
            }
            else
            {
                // randomly choose one of two neighbors
                otherRiverBank.Add(localSharedTiles[Generator.random.Next(0, 2)]);
            }
        }
        if (otherRiverBank.Count == 0)
        {
            throw new Exception("Error: special case for otherRiverBank is empty.");
        }
        else
        {
            // for all other tiles in riverPath
            for(int i = 0; i < riverPath.Count; ++i)
            {
                int maxTry = 5;
                int tryCount = 0;
                do
                {
                    var otherRiverBankNeighbors = otherRiverBank[^1].Neighbors(grid.Cast<HexTile>().ToList(), rows, columns);
                    // filter out all river tiles
                    otherRiverBankNeighbors = otherRiverBankNeighbors.Except(riverPath).ToList();
                    sharedTiles = Utils.FindCommonTiles(new List<List<Tile>>() { sharedTiles, riverPath });
                    foreach(var sharedTile in sharedTiles)
                    {
                        if(sharedTile.terrain != TerrainType.SHALLOW_WATER && sharedTile.Coordinates != mountain.Coordinates)
                        {
                            if(!otherRiverBank.Contains(sharedTile))
                            {
                                otherRiverBank.Add(sharedTile);
                            }
                        }
                    }
                    ++tryCount;
                } while (tryCount < maxTry);
            }
        }
        for(int i = 0; i < otherRiverBank.Count; ++i)
        {
            otherRiverBank[i].river = RiverType.RIVERBANK;
            riverPath.Add(otherRiverBank[i]);
        }
    }

    // generates a map of coordinates and infos which river tiles should be added (NE, E, SE, SW, W, NW)
    internal static Dictionary<CubeCoordinates, List<Direction>> GenerateRiverTileDirections(List<Tile> riverTiles)
    {
        // create empty dictionary
        Dictionary<CubeCoordinates, List<Direction>> riverDirections = new();
        for (int i = 0; i < riverTiles.Count; ++i)
        {
            for (int j = 0; j < riverTiles.Count; ++j)
            {
                if (i == j)
                {
                    continue;
                }
                if (riverTiles[i].river != riverTiles[j].river)
                {
                    List<Direction> neighborDirections = new();
                    var key = riverTiles[i].Coordinates;
                    if (riverDirections.ContainsKey(key))
                    {
                        neighborDirections = riverDirections[key];
                    }
                    var direction = DetectNeighborhood(riverTiles[i], riverTiles[j]);
                    if (direction is not null && !neighborDirections.Contains(direction.Value))
                    {
                        neighborDirections.Add(direction.Value);
                    }
                    riverDirections[key] = neighborDirections;
                }
            }
        }
        return riverDirections;
    }

    // detects if two tiles are neighbors and returns the direction from target point of view, undefined if not a neighbor
    internal static Direction? DetectNeighborhood(Tile source, Tile target)
    {
        int q = target.Coordinates.q - source.Coordinates.q;
        int r = target.Coordinates.r - source.Coordinates.r;
        int s = target.Coordinates.s - source.Coordinates.s;
        if(q == 1 && r == -1 && s == 0)
        {
            return Direction.NE;
        }
        if (q == 1 && r == 0 && s == -1)
        {
            return Direction.E;
        }
        if (q == 0 && r == 1 && s == -1)
        {
            return Direction.SE;
        }
        if (q == -1 && r == 1 && s == 0)
        {
            return Direction.SW;
        }
        if (q == -1 && r == 0 && s == 1)
        {
            return Direction.W;
        }
        if (q == 0 && r == -1 && s == 1)
        {
            return Direction.NW;
        }
        return null;
    }
}
