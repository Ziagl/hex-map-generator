using com.hexagonsimulations.HexMapGenerator.Enums;
using com.hexagonsimulations.HexMapGenerator.Models;
using System.Text.Json;

namespace com.hexagonsimulations.HexMapGenerator;

public class Converter
{
    // all tilesets (Terrain, Landscape and River) are numbered individually (1...X)
    // if this is set, it assumes there is only one big TileSet numbered continuously
    // (landscape 1 = last Terrain tile + 1)
    private readonly bool _combinedTileSet = false;

    public Converter()
    {

    }

    public Converter (bool combinedTileSet)
    {
        _combinedTileSet = combinedTileSet;
    }

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
                    data = map.LandscapeMap;
                    if(_combinedTileSet)
                    {
                        for(int x = 0; x < data.Count; ++x)
                        {
                            if (data[x] > 0)
                            {
                                data[x] += (int)TerrainType.MOUNTAIN;
                            }
                        }
                    }
                    break;
                case 2:
                    data = map.RiverMap; 
                    if(_combinedTileSet)
                    {
                        for (int x = 0; x < data.Count; ++x)
                        {
                            if (data[x] == 1)
                            {
                                data[x] += (int)TerrainType.MOUNTAIN + (int)LandscapeType.VOLCANO;
                            }
                        }
                    }
                    break;
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
