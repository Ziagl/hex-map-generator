namespace com.hexagonsimulations.HexMapGenerator.Enums;

public enum RiverType
{
    NONE = 0,
    RIVERBANK = 99, // other side of river
    RIVERAREA = 98, // distance around riverbed not other river can be added
    RIVER = 1,
}
