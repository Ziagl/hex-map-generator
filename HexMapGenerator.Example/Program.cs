// See https://aka.ms/new-console-template for more information
using HexMapGenerator;
using HexMapGenerator.Enums;

// generator values
var mapSize = MapSize.SMALL;
var mapType = MapType.HIGHLAND;
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

var generator = new Generator(1234);    // add a seed to get the same map everytime
generator.GenerateMap(mapType, mapSize, mapTemperature, mapHumidity, factorRiver);

Console.WriteLine(generator.Print());

var converter = new Converter(true);
string json = converter.GenerateTiledJson(generator.MapData, imagefile, tileWidth, tileHeight, imageWidth, imageHeight, tileCount, tileColumns, transparentColor);
Console.WriteLine("Json:" + json);
File.WriteAllText("mapData.json", json);

//Console.WriteLine(json);
