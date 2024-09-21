namespace HexMapGenerator.models;

public record TileSet
{
    public int columns { get; init; } = 0;
    public int firstgid { get; init; } = 1;
    public string image { get; init; } = string.Empty;
    public int imageheight { get; init; } = 0;
    public int imagewidth { get; init; } = 0;
    public int margin { get; init; } = 0;
    public string name { get; init; } = "";
    public int spacing { get; init; } = 0;
    public int tilecount { get; init; } = 0;
    public int tileheight { get; init; } = 0;
    public int tilewidth { get; init; } = 0;
    public string transparentcolor { get; init; } = "";
}
