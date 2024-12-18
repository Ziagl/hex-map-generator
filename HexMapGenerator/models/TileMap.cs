namespace com.hexagonsimulations.HexMapGenerator.Models;

public record TileMap
{
    public int compressionlevel { get; init; } = -1;
    public int height { get; init; } = 0;
    public int hexsidelength { get; init; } = 0;
    public bool infinite { get; init; } = false;
    public List<TileLayer> layers { get; init; } = new();
    public int nextlayerid { get; init; } = 2;
    public int nextobjectid { get; init; } = 1;
    public string orientation { get; init; } = "hexagonal";
    public string renderorder { get; init; } = "right-down";
    public string staggeraxis { get; init; } = "y";
    public string staggerindex { get; init; } = "odd";
    public string tiledversion { get; init; } = "1.11.0";
    public int tileheight { get; init; } = 0;
    public List<TileSet> tilesets { get; init; } = new();
    public int tilewidth { get; init; } = 0;
    public string type { get; init; } = "map";
    public string version { get; init; } = "1.1";
    public int width { get; init; } = 0;
}
