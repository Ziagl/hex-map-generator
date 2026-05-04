namespace com.hexagonsimulations.HexMapGenerator.Enums;

public enum LandscapeType
{
    NONE = 0, // nothing special
    ICE = 1, // water specific arctic
    OASIS = 2, // desert specific
    SWAMP = 3, // plain / grass specific

    // map resources
    FOREST = 4, // plain / grass / tundra specific
    JUNGLE = 5, // plain / grass specific tropical
    STONE = 6, // plain / grass / tundra / snow hills specific
}
