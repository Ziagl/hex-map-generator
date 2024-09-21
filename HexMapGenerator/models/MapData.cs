namespace HexMapGenerator.models;

public record MapData
{
    public List<int> TerrainMap = new List<int>();
    public int Rows;
    public int Columns;
}
