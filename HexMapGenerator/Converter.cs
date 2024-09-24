using HexMapGenerator.models;
using System.Text.Json;

namespace HexMapGenerator;

public class Converter
{
    public string GenerateTiledJson(MapData map, string imagefile, int tileWidth, int tileHeight, int imageWidth, int imageHeight, int tileCount, int tileColumns, string transparentColor)
    {
        TileMap tileMap = new()
        {
            width = map.Columns,
            height = map.Rows,
            tilewidth = tileWidth,
            tileheight = tileHeight,
            hexsidelength = tileWidth / 2,
        };

        // add all layers
        for (int i = 0; i < 3; i++)
        {
            List<int> data = new();
            switch (i)
            {
                case 0:
                    data = map.TerrainMap; break;
                case 1:
                    data = map.LandscapeMap; break;
                case 2:
                    data = map.RiverMap; break;
            }

            TileLayer layer = new()
            {
                width = map.Columns,
                height = map.Rows,
                name = "generated tile layer " + i,
                data = data,
            };

            tileMap.layers.Add(layer);
        }

        TileSet tileset = new()
        {
            imageheight = imageHeight,
            imagewidth = imageWidth,
            columns = tileColumns,
            tilecount = tileCount,
            tilewidth = tileWidth,
            tileheight = tileHeight,
            image = imagefile,
            transparentcolor = transparentColor,
        };

        tileMap.tilesets.Add(tileset);

        return JsonSerializer.Serialize(tileMap, new JsonSerializerOptions { WriteIndented = true });
    }
}
