namespace HexMapGenerator.models;

public record MapData
{
    public List<int> TerrainMap = new();
    public List<int> LandscapeMap = new();
    public List<int> RiverMap = new();
    public Dictionary<string, string> riverTileDirections = new();
    public int Rows;
    public int Columns;
}
