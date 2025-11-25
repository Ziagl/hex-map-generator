# AGENTS.md - Hex Map Generator Library

## Project Overview

**HexMapGenerator** is a C# .NET library for procedurally generating hexagonal tile-based maps compatible with [Tiled Map Editor](https://www.mapeditor.org/). The library generates diverse terrain layouts with configurable climate parameters and exports to Tiled JSON format.

**Key Features:**
- 10 distinct terrain generation algorithms (continents, archipelago, islands, highland, etc.)
- Climate-aware generation (temperature, humidity)
- Three-layer map structure (terrain, landscape features, rivers)
- Seed-based reproducible generation
- JSON and binary serialization support
- Tiled Map Editor integration

**Technology Stack:**
- .NET 10.0 / .NET 8.0
- C# with nullable reference types enabled
- Dependencies: HexMapBase (v0.5.0)
- Testing: MSTest framework

## Architecture

### Core Components

#### 1. Generator (`Generator.cs`)
The main entry point for map generation. Orchestrates the two-phase generation process:

```csharp
var generator = new Generator();  // Random seed
var generator = new Generator(1234);  // Fixed seed for reproducibility

generator.GenerateMap(
    MapType.CONTINENTS,
    MapSize.SMALL,
    MapTemperature.NORMAL,
    MapHumidity.NORMAL,
    factorRiver: 2.0f  // Rivers per map size multiplier
);

MapData mapData = generator.MapData;
```

**Generation Phases:**
1. **Terrain Generation** - Creates base landmasses and water bodies via `IMapTerrainGenerator`
2. **Landscape Shaping** - Adds climate-specific details via `IMapLandscapeShaper`

#### 2. Converter (`Converter.cs`)
Transforms `MapData` into Tiled Map Editor JSON format:

```csharp
var converter = new Converter();  // Separate tilesets
var converter = new Converter(combinedTileSet: true);  // Single tileset

string json = converter.GenerateTiledJson(
    mapData,
    imagefile: "tileset.png",
    tileWidth: 32,
    tileHeight: 34,
    imageWidth: 1536,
    imageHeight: 34,
    tileCount: 48,
    tileColumns: 48,
    transparentColor: "#ffffff"
);

File.WriteAllText("mapData.json", json);
```

**Tileset Modes:**
- **Separate** (default): Terrain (1-13), Landscape (1-7), River (1) numbered independently
- **Combined**: Continuous numbering (Landscape starts at 14, River at 21)

#### 3. MapData Model (`models/MapData.cs`)
Central data structure containing all map information:

```csharp
public record MapData
{
    // Tile layers (row-major indexed: y * Columns + x)
    public List<int> TerrainMap { get; set; }      // Base terrain types
    public List<int> LandscapeMap { get; set; }    // Features (forests, volcanoes, etc.)
    public List<int> RiverMap { get; set; }        // River presence

    // River flow directions (cube coordinates → edge directions)
    public Dictionary<CubeCoordinates, List<Direction>> RiverTileDirections { get; set; }

    // Map dimensions
    public int Rows { get; set; }
    public int Columns { get; set; }

    // Generation parameters
    public MapType Type { get; set; }
    public MapSize Size { get; set; }
    public MapTemperature Temperature { get; set; }
    public MapHumidity Humidity { get; set; }

    // Serialization
    public string ToJson();
    public static MapData FromJson(string json);
    public void Write(BinaryWriter writer);
    public static MapData Read(BinaryReader reader);
}
```

**Coordinate System:**
- Storage: Row-major 1D array (`index = row * Columns + column`)
- Working: Cube coordinates for hexagonal operations (`CubeCoordinates`, `OffsetCoordinates`)
- Conversion utilities in `HexMapBase` package

### Map Generation System

#### Map Types (`enums/MapType.cs`)

Each type uses a specialized generator in `generators/` folder:

| MapType | Description | Generator Class |
|---------|-------------|----------------|
| `RANDOM` | Testing/random generation | `RandomGenerator` |
| `ARCHIPELAGO` | Many small islands | `ArchipelagoGenerator` |
| `INLAND_SEA` | Plains with central sea | `InlandSeaGenerator` |
| `HIGHLAND` | Hills, mountains, lakes | `HighlandGenerator` |
| `ISLANDS` | Various sized islands | `IslandsGenerator` |
| `SMALL_CONTINENTS` | Equal-sized landmasses | `SmallContinentsGenerator` |
| `CONTINENTS` | Few large continents | `ContinentsGenerator` |
| `CONTINENTS_ISLANDS` | Continents + islands | `ContinentsIslandsGenerator` |
| `SUPER_CONTINENT` | Single giant landmass | `SuperContinentGenerator` |
| `LAKES` | Plains with numerous lakes | `LakesGenerator` |

#### Map Sizes (`enums/MapSize.cs`)

Predefined dimensions optimized for civilization counts:

| Size | Dimensions | Tiles | Civilizations |
|------|-----------|-------|---------------|
| `MICRO` | 26×44 | 1,144 | 2-4 |
| `TINY` | 38×60 | 2,280 | 4-6 |
| `SMALL` | 46×74 | 3,404 | 6-8 |
| `MEDIUM` | 54×84 | 4,536 | 8-14 |
| `LARGE` | 60×96 | 5,760 | 10-16 |
| `HUGE` | 66×106 | 6,996 | 12-20 |

#### Climate Parameters

**MapTemperature** (`enums/MapTemperature.cs`)
- `HOT` - More desert, jungle; less snow/tundra
- `NORMAL` - Balanced distribution
- `COLD` - More snow, tundra; less tropical features

**MapHumidity** (`enums/MapHumidity.cs`)
- `DRY` - More desert, less forest/grass
- `NORMAL` - Balanced distribution
- `WET` - More forest, jungle, swamps

Climate affects polar ice extent, vegetation distribution, and desert placement.

### Three-Layer Map Structure

#### Layer 1: Terrain (`enums/TerrainType.cs`)
Base tile types representing elevation and base biome:

```csharp
public enum TerrainType
{
    DEEP_WATER = 1,      // Ocean, surrounded by water
    SHALLOW_WATER = 2,   // Coastal water
    DESERT = 3,          // Sand terrain
    DESERT_HILLS = 4,    // Hilly desert
    PLAIN = 5,           // Flat land
    PLAIN_HILLS = 6,     // Hilly plains
    GRASS = 7,           // Grassland
    GRASS_HILLS = 8,     // Hilly grassland
    TUNDRA = 9,          // Cold plains
    TUNDRA_HILLS = 10,   // Cold hills
    SNOW = 11,           // Arctic plains
    SNOW_HILLS = 12,     // Arctic hills
    MOUNTAIN = 13,       // Impassable peaks
}
```

**Generation Pattern:**
1. Initialize base terrain (usually SHALLOW_WATER or PLAIN)
2. Use seed-based expansion for land/water formation
3. Add elevation (hills, mountains)
4. Apply climate transformations (snow, tundra, grass, desert)

#### Layer 2: Landscape Features (`enums/LandscapeType.cs`)
Decorative features placed on terrain:

```csharp
public enum LandscapeType
{
    NONE = 0,      // No feature
    ICE = 1,       // Arctic water (on WATER tiles)
    REEF = 2,      // Tropical water (on DEEP_WATER)
    OASIS = 3,     // Desert feature (on DESERT)
    SWAMP = 4,     // Wetland (on PLAIN/GRASS/TUNDRA)
    FOREST = 5,    // Temperate trees (on PLAIN/GRASS/TUNDRA + HILLS)
    JUNGLE = 6,    // Tropical trees (on PLAIN/GRASS + HILLS, equatorial)
    VOLCANO = 7,   // Rare feature (on MOUNTAIN)
}
```

**Placement Rules:**
- Climate-dependent: ICE/JUNGLE based on latitude and temperature
- Terrain-restricted: Each type has compatible terrain list
- Distribution-based: Uses `TileDistribution` for latitude weighting

#### Layer 3: Rivers (`enums/RiverType.cs`)
Water flow system with directional data:

```csharp
public enum RiverType
{
    NONE = 0,       // No river
    RIVER = 1,      // River tile
    RIVERAREA = 2,  // Internal: buffer zone (converted to NONE)
}
```

**River Generation Algorithm:**
1. Select mountain tiles far from map edges
2. Calculate distance to nearest SHALLOW_WATER
3. Create downhill path using A* or similar pathfinding
4. Enforce minimum length (default: 3 tiles)
5. Prevent overlapping via riverbed buffer zones
6. Store flow directions in `RiverTileDirections` dictionary

**Key Parameters:**
- `factorRiver`: Multiplier for river count (e.g., 2.0 = 2 rivers per map size level)
- `_riverbed`: Buffer distance preventing adjacent rivers (default: 3)
- `minRiverLength`: Minimum tiles for valid river (default: 3)

### Extension Points

#### Creating Custom Terrain Generators

Implement `IMapTerrainGenerator` interface:

```csharp
namespace com.hexagonsimulations.HexMapGenerator.Interfaces;

internal interface IMapTerrainGenerator
{
    void Generate(MapData data);
}
```

**Example Pattern (from `ContinentsGenerator.cs`):**

```csharp
internal class CustomGenerator : IMapTerrainGenerator
{
    private readonly float _factorLand = 0.7f;  // Land percentage

    public void Generate(MapData map)
    {
        // 1. Initialize grid
        List<Tile> grid = Enumerable.Repeat(new Tile(), map.Rows * map.Columns).ToList();
        Utils.InitializeHexGrid(grid, map.Rows, map.Columns, TerrainType.SHALLOW_WATER);

        // 2. Place seeds for landmasses
        Utils.AddRandomContinentSeed(grid, map.Rows, map.Columns, TerrainType.SHALLOW_WATER, continentCount);

        // 3. Expand seeds to form continents
        // Use Utils.RandomNeighbors, Utils.Shuffle for stochastic growth

        // 4. Add elevation features
        Utils.AddRandomTileSeed(grid, map.Rows, map.Columns, mountainTiles, /* ... */);
        Utils.ExpandHills(grid, map.Rows, map.Columns, mountainTiles, hillTiles);
        Utils.HillsToMountains(grid, map.Rows, map.Columns, mountainCount);

        // 5. Finalize water depth
        Utils.ShallowToDeepWater(grid, map.Rows, map.Columns);

        // 6. Convert to MapData
        map.TerrainMap = Utils.ConvertGrid(grid);
    }
}
```

**Register in `Generator.cs`:**
```csharp
case MapType.CUSTOM:
    generator = new CustomGenerator();
    break;
```

#### Creating Custom Landscape Shapers

Implement `IMapLandscapeShaper` interface:

```csharp
namespace com.hexagonsimulations.HexMapGenerator.Interfaces;

internal interface IMapLandscapeShaper
{
    void Generate(MapData data, float factorRiver, int riverBed);
}
```

**Example Pattern (from `DefaultShaper.cs`):**

```csharp
internal class CustomShaper : IMapLandscapeShaper
{
    public void Generate(MapData map, float factorRiver, int riverBed)
    {
        // 1. Initialize grid from TerrainMap
        List<Tile> grid = new();
        for (int column = 0; column < map.Columns; ++column)
        {
            for (int row = 0; row < map.Rows; ++row)
            {
                int index = row * map.Columns + column;
                grid[index] = new Tile
                {
                    Coordinates = new OffsetCoordinates(column, row).ToCubic(),
                    terrain = (TerrainType)map.TerrainMap[index],
                    landscape = LandscapeType.NONE,
                    river = RiverType.NONE,
                };
            }
        }

        // 2. Generate rivers (optional)
        int riverCount = (int)(factorRiver * ((int)map.Size + 1));
        var rivers = ComputeRivers(grid, map.Rows, map.Columns, riverCount, 3, riverBed);

        // 3. Apply climate effects
        CreateSnowTiles(grid, map.Rows, map.Temperature);
        CreateTundraTiles(grid, map.Rows, map.Temperature);

        // 4. Add vegetation and features
        Utils.AddRandomTerrain(grid, map.Rows, map.Columns, TerrainType.GRASS, TerrainType.GRASS_HILLS, count, distribution);
        Utils.AddRandomLandscape(grid, map.Rows, map.Columns, LandscapeType.FOREST, affectedTerrain, count, distribution);

        // 5. Export to MapData
        map.LandscapeMap = grid.Select(t => (int)t.landscape).ToList();
        map.RiverMap = grid.Select(t => (int)t.river).ToList();
        map.TerrainMap = Utils.ConvertGrid(grid);
        map.RiverTileDirections = /* combine river direction dictionaries */;
    }
}
```

### Utility Functions (`models/Utils.cs`)

Critical helper methods for map generation (1000+ lines):

**Grid Initialization:**
- `InitializeHexGrid()` - Fill grid with base terrain
- `ConvertGrid()` - Convert Tile list to int list for MapData

**Seed Placement:**
- `AddRandomContinentSeed()` - Place numbered continent origins
- `AddRandomTileSeed()` - Place landmass/feature seeds

**Expansion Algorithms:**
- `ExpandWater()` - Grow lakes/oceans from seeds
- `ExpandHills()` - Grow hill formations
- `RandomNeighbors()` - Get shuffled neighbor tiles for stochastic growth
- `Shuffle<T>()` - Fisher-Yates shuffle for randomness

**Terrain Transformation:**
- `ShallowToDeepWater()` - Convert surrounded water to deep
- `HillsToMountains()` - Upgrade clustered hills to mountains
- `AddRandomTerrain()` - Place terrain with latitude distribution
- `AddRandomLandscape()` - Place landscape features with distribution

**River Algorithms:**
- `CreateRiverPath()` - Pathfind from mountain to water
- `ExtendRiverPath()` - Widen river to multi-tile
- `GenerateRiverTileDirections()` - Calculate flow directions
- `DistanceToRiver()` - Distance calculation for buffering

**Coordinate Utilities:**
- `IsTileAtEdge()` - Check proximity to map boundary
- `FindNearestTile()` - Distance to specific terrain type
- `CountTiles()` - Count terrain type occurrences

**Distribution System:**
`TileDistribution` struct controls latitude-based placement:
```csharp
// (north, mid-north, mid-south, south) weights
var distribution = new TileDistribution(0.0f, 0.5f, 0.1f, 0.4f);
// 0% north, 50% mid-north, 10% mid-south, 40% south
```

### Testing Approach

#### Test Structure (`HexMapGenerator.Tests/`)

**Generator Tests (`GeneratorTests.cs`):**
- Test all 10 map types generate valid data
- Verify layer dimensions match rows × columns
- Test river generation with seed reproducibility

```csharp
[TestMethod]
public void TestContinentsGenerator()
{
    var generator = new Generator();
    generator.GenerateMap(MapType.CONTINENTS, MapSize.TINY, 
                         MapTemperature.HOT, MapHumidity.DRY, 0.0f);
    
    Assert.AreEqual(generator.MapData.TerrainMap.Count, 
                   generator.MapData.Rows * generator.MapData.Columns);
    Assert.AreEqual(generator.MapData.LandscapeMap.Count, 
                   generator.MapData.Rows * generator.MapData.Columns);
}
```

**Converter Tests (`ConverterTests.cs`):**
- Validate Tiled JSON structure
- Test separate vs. combined tileset modes
- Verify layer numbering correctness

**Serialization Tests (`MapDataSerializationTests.cs`):**
- JSON round-trip (serialize → deserialize → compare)
- Binary round-trip
- Validate RiverTileDirections preservation

**Utility Tests (`UtilsTests.cs`, `MapUtilsTests.cs`):**
- Test coordinate conversions
- Validate map size calculations
- Test neighbor finding algorithms

#### Running Tests

```powershell
# Run all tests
dotnet test HexMapGenerator.Tests/HexMapGenerator.Tests.csproj

# Run specific test
dotnet test --filter "TestMethod=TestRiver"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Project Structure

```
hex-map-generator/
├── HexMapGenerator/              # Main library
│   ├── Generator.cs              # Main generation orchestrator
│   ├── Converter.cs              # Tiled JSON exporter
│   ├── enums/                    # Public enumerations
│   │   ├── MapType.cs
│   │   ├── MapSize.cs
│   │   ├── MapTemperature.cs
│   │   ├── MapHumidity.cs
│   │   ├── TerrainType.cs
│   │   ├── LandscapeType.cs
│   │   └── RiverType.cs
│   ├── models/                   # Data models
│   │   ├── MapData.cs            # Core data structure
│   │   ├── Tile.cs               # Working tile representation
│   │   ├── TileDistribution.cs   # Latitude distribution config
│   │   ├── TileMap.cs            # Tiled JSON models
│   │   └── Utils.cs              # Generation utilities (internal)
│   ├── generators/               # Terrain generators (internal)
│   │   ├── ArchipelagoGenerator.cs
│   │   ├── ContinentsGenerator.cs
│   │   ├── ContinentsIslandsGenerator.cs
│   │   ├── HighlandGenerator.cs
│   │   ├── InlandSeaGenerator.cs
│   │   ├── IslandsGenerator.cs
│   │   ├── LakesGenerator.cs
│   │   ├── RandomGenerator.cs
│   │   ├── SmallContinentsGenerator.cs
│   │   └── SuperContinentGenerator.cs
│   ├── shapers/                  # Landscape shapers (internal)
│   │   └── DefaultShaper.cs
│   ├── interfaces/               # Internal interfaces
│   │   ├── IMapTerrainGenerator.cs
│   │   └── IMapLandscapeShaper.cs
│   └── HexMapGenerator.csproj
├── HexMapGenerator.Example/      # Usage examples
│   ├── Program.cs                # Sample implementation
│   └── HexMapGenerator.Example.csproj
├── HexMapGenerator.Tests/        # Unit tests
│   ├── GeneratorTests.cs
│   ├── ConverterTests.cs
│   ├── MapDataSerializationTests.cs
│   ├── UtilsTests.cs
│   └── HexMapGenerator.Tests.csproj
├── README.md                     # User documentation
├── LICENSE                       # MIT License
└── HexMapGenerator.sln           # Solution file
```

### Dependencies

**HexMapBase (v0.5.0)** - Hexagonal geometry library providing:
- `CubeCoordinates` - Cube coordinate system for hex math
- `OffsetCoordinates` - Offset (x, y) coordinate system
- `Direction` enum - Hexagonal directions (NE, E, SE, SW, W, NW)
- `HexTile` - Base tile with neighbor finding
- Coordinate conversion utilities
- Spiral and ring iteration methods

### Common Patterns

#### Seed-Based Reproducibility

```csharp
// Always get the same map
var generator = new Generator(12345);
generator.GenerateMap(MapType.CONTINENTS, MapSize.MEDIUM, 
                     MapTemperature.NORMAL, MapHumidity.NORMAL, 1.0f);

// Random each time
var randomGenerator = new Generator();
```

The seed initializes `Generator.random` (static Random instance) used throughout generation.

#### Stochastic Expansion

Many generators use this pattern:
1. Place random seeds
2. Shuffle neighbor order
3. Randomly expand into valid neighbors
4. Repeat until target tile count reached

```csharp
do
{
    var tile = Utils.RandomTile(grid, rows, columns);
    var neighbors = Utils.RandomNeighbors(grid, rows, columns, tile);
    foreach (var neighbor in neighbors)
    {
        if (IsValidExpansion(neighbor))
        {
            neighbor.terrain = TerrainType.PLAIN;
            --remainingTiles;
        }
    }
} while (remainingTiles > 0 && loopMax-- > 0);
```

#### Climate-Based Transformation

Apply latitude-based effects using map rows:

```csharp
// Polar regions (top/bottom rows)
if (tile.Coordinates.r == 0 || tile.Coordinates.r == rows - 1)
{
    if (tile.terrain == TerrainType.PLAIN)
        tile.terrain = TerrainType.SNOW;
}

// Temperature modifier
int chanceMultiplier = temperature == MapTemperature.COLD ? 2 : 1;
```

#### Percentage-Based Distribution

```csharp
int totalLandTiles = Utils.CountTiles(grid, landTerrainTypes);
int forestTiles = (int)(totalLandTiles * 0.3f * humidityFactor);

var distribution = new TileDistribution(
    north: 0.1f,      // 10% in northern quarter
    midNorth: 0.4f,   // 40% in mid-north quarter
    midSouth: 0.4f,   // 40% in mid-south quarter
    south: 0.1f       // 10% in southern quarter
);

Utils.AddRandomLandscape(grid, rows, columns, LandscapeType.FOREST, 
                        affectedTerrain, forestTiles, distribution);
```

### Important Constraints

**Map Generation:**
- All generators must populate `map.TerrainMap` with valid `TerrainType` values (1-13)
- Grid indices are row-major: `index = row * columns + column`
- Coordinate conversions required when working with hexagonal neighbors
- Loop safety: Use `MAXLOOPS` constant to prevent infinite loops in stochastic processes

**River Generation:**
- Rivers start from mountains not near edges (`IsTileAtEdge` check)
- Minimum river length prevents trivial rivers
- Riverbed buffer prevents rivers from being too close
- `RiverTileDirections` uses cube coordinates as keys, not offset indices
- RIVERAREA tiles are internal markers, converted to NONE in final output

**Landscape Placement:**
- Each `LandscapeType` has specific compatible `TerrainType` list
- Features cannot be placed on incompatible terrain
- Latitude distribution respects map temperature/humidity
- Multiple features cannot occupy same tile (last wins)

**Tiled Export:**
- Tile indices in JSON are 1-based (0 = empty)
- Combined tileset mode requires offset calculations
- Three layers must have identical dimensions
- Hexagonal properties (`hexsidelength`) required for proper rendering

### Debugging and Troubleshooting

**Common Issues:**

1. **Map Generation Hangs:**
   - Check loop counters (MAXLOOPS)
   - Verify expansion conditions aren't impossible
   - Use smaller map sizes during development

2. **Rivers Not Appearing:**
   - Ensure `factorRiver > 0`
   - Verify mountains exist far enough from edges
   - Check minimum river length requirement
   - Inspect `RiverTileDirections` dictionary population

3. **Incorrect Terrain Distribution:**
   - Verify percentage factors sum reasonably
   - Check `TileDistribution` weights (should sum to 1.0)
   - Confirm temperature/humidity multipliers applied

4. **Tiled Rendering Issues:**
   - Verify tileset image dimensions match parameters
   - Check tile width/height consistency
   - Ensure `hexsidelength = tileWidth / 2`
   - Validate combined vs. separate tileset mode

**Print Debugging:**

```csharp
// Text representation
Console.WriteLine(generator.Print());

// JSON inspection
Console.WriteLine(mapData.ToJson());

// Tile counts
foreach (TerrainType terrain in Enum.GetValues<TerrainType>())
{
    int count = mapData.TerrainMap.Count(t => t == (int)terrain);
    Console.WriteLine($"{terrain}: {count}");
}
```

### Performance Considerations

**Generation Time:**
- MICRO/TINY: < 100ms
- SMALL/MEDIUM: 100-500ms
- LARGE/HUGE: 500ms-2s

**Optimization Tips:**
- Use fixed seeds during testing to avoid regeneration
- Cache `MapData` serialization for repeated use
- Limit river generation on huge maps
- Profile stochastic loops if generation is slow

**Memory Usage:**
- Working grid: ~80 bytes per tile (Tile object)
- MapData: ~12 bytes per tile (3 × int32)
- HUGE map: ~560KB for MapData, ~6MB for working grid

### Future Extension Ideas

**New Generators:**
- Fractal-based generation for more realistic coastlines
- Tectonic plate simulation
- Erosion and weathering simulation
- Biome-first approach (define climate zones, then terrain)

**Enhanced Features:**
- Multiple river sizes (streams, rivers, major rivers)
- River deltas and estuaries
- Mountain ranges with coherent orientation
- Natural resource placement (strategic resources)
- Road/path generation between settlements

**Advanced Climate:**
- Seasonal variation data
- Rainfall patterns
- Wind systems
- Ocean currents affecting climate

**Export Formats:**
- Unity tilemap format
- Godot tilemap format
- Custom binary format for faster loading
- PNG heightmap export

## Quick Reference

### Minimal Usage Example

```csharp
using com.hexagonsimulations.HexMapGenerator;
using com.hexagonsimulations.HexMapGenerator.Enums;

// Generate
var generator = new Generator(1234);
generator.GenerateMap(MapType.CONTINENTS, MapSize.SMALL, 
                     MapTemperature.NORMAL, MapHumidity.NORMAL, 2.0f);

// Export
var converter = new Converter(true);
string json = converter.GenerateTiledJson(
    generator.MapData, "tileset.png", 32, 34, 1536, 34, 48, 48, "#ffffff"
);
File.WriteAllText("map.json", json);
```

### Serialization Example

```csharp
// Save
string json = mapData.ToJson();
File.WriteAllText("map.json", json);

using var fs = File.Create("map.bin");
using var writer = new BinaryWriter(fs);
mapData.Write(writer);

// Load
string json = File.ReadAllText("map.json");
var loadedMapData = MapData.FromJson(json);

using var fs = File.OpenRead("map.bin");
using var reader = new BinaryReader(fs);
var loadedMapData = MapData.Read(reader);
```

### Key Enum Values

**MapType:** RANDOM, ARCHIPELAGO, INLAND_SEA, HIGHLAND, ISLANDS, SMALL_CONTINENTS, CONTINENTS, CONTINENTS_ISLANDS, SUPER_CONTINENT, LAKES

**MapSize:** MICRO, TINY, SMALL, MEDIUM, LARGE, HUGE

**MapTemperature:** HOT, NORMAL, COLD

**MapHumidity:** DRY, NORMAL, WET

**TerrainType:** DEEP_WATER(1), SHALLOW_WATER(2), DESERT(3-4), PLAIN(5-6), GRASS(7-8), TUNDRA(9-10), SNOW(11-12), MOUNTAIN(13)

**LandscapeType:** NONE(0), ICE(1), REEF(2), OASIS(3), SWAMP(4), FOREST(5), JUNGLE(6), VOLCANO(7)

**RiverType:** NONE(0), RIVER(1)

## AI Agent Guidelines

**When modifying this codebase:**

1. **Preserve Randomness:** Maintain `Generator.random` usage for reproducibility. Don't introduce separate Random instances.

2. **Respect Interfaces:** New generators/shapers must implement `IMapTerrainGenerator`/`IMapLandscapeShaper` exactly.

3. **Test All Map Types:** Changes to core utilities affect all 10 generators. Run full test suite.

4. **Coordinate Systems:** Always convert between offset (storage) and cube (hexagonal operations) coordinates properly.

5. **Maintain Layer Integrity:** All three layers (terrain, landscape, river) must remain same size and synchronized.

6. **Document Constants:** Magic numbers for percentages, ranges, or thresholds should be named constants with comments.

7. **Loop Safety:** Always include loop counters (`MAXLOOPS`) for stochastic processes to prevent hangs.

8. **Backward Compatibility:** This is a published NuGet package. Breaking changes to public API require version bump.

**When debugging:**
- Use `generator.Print()` for quick visual inspection
- Check `MapData.ToJson()` for detailed state examination
- Verify tile counts match expected percentages
- Test with fixed seed before using random generation

**When adding features:**
- Add new enum values to end of existing enums (preserve binary compatibility)
- Create unit tests in `HexMapGenerator.Tests`
- Update `README.md` with user-facing documentation
- Consider serialization impact if modifying `MapData`

---

**Version:** 0.5.0  
**Last Updated:** November 25, 2025  
**Repository:** https://github.com/Ziagl/hex-map-generator  
**Package:** https://www.nuget.org/packages/HexMapGenerator  
**License:** MIT
