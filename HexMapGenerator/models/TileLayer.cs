namespace HexMapGenerator.Models;

public record TileLayer
{
    public List<int> data { get; init; } = new();
    public int height { get; init; } = 0;
    public int id { get; init; } = 1;
    public string name { get; init; } = "";
    public int opacity { get; init; } = 1;
    public string type { get; init; } = "tilelayer";
    public bool visible { get; init; } = true;
    public int width { get; init; } = 0;
    public int x { get; init; } = 0;
    public int y { get; init; } = 0;
}
