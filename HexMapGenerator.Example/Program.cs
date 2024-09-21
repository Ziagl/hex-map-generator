// See https://aka.ms/new-console-template for more information
using HexMapGenerator;
using HexMapGenerator.enums;

// generator values
var mapSize = MapSize.SMALL;
var mapType = MapType.SUPER_CONTINENT;
var mapTemperature = MapTemperature.NORMAL;
var mapHumidity = MapHumidity.NORMAL;
float factorRiver = 0.0f;

// converter values
string imagefile = "tileset.png";
int tileWidth = 32;
int tileHeight = 34;
int imageWidth = 1536;
int imageHeight = tileHeight;
int tileCount = imageWidth / tileWidth;
int tileColumns = tileCount;
string transparentColor = "#ffffff";

Console.WriteLine("Generating new map");

var generator = new Generator();
generator.GenerateMap(mapType, mapSize, mapTemperature, mapHumidity, factorRiver);

Console.WriteLine(generator.Print());

var converter = new Converter();
string json = converter.GenerateTiledJson(generator.MapData, imagefile, tileWidth, tileHeight, imageWidth, imageHeight, tileCount, tileColumns, transparentColor);
Console.WriteLine("Json:" + json);
File.WriteAllText("mapData.json", json);

//Console.WriteLine(json);
