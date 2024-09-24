# hex-map-generator
A hexagon map generator for Tiled Map Editor (https://www.mapeditor.org/) files.

## Sample of generated maps inside Tiled Map Editor:

Type: CONTINENTS_ISLANDS, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/continents_islands-tiny.png "Type: CONTINENTS_ISLANDS, Size: TINY")

## Sample code to create map

```csharp
using HexMapGenerator;
using HexMapGenerator.enums;

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

var generator = new Generator();
generator.GenerateMap(mapType, mapSize, mapTemperature, mapHumidity, factorRiver);

Console.WriteLine(generator.Print());

var converter = new Converter();
string json = converter.GenerateTiledJson(generator.MapData, imagefile, tileWidth, tileHeight, imageWidth, imageHeight, tileCount, tileColumns, transparentColor);
Console.WriteLine("Json:" + json);
File.WriteAllText("mapData.json", json);
```

## All possible map types as example images:

Type: ARCHIPELAGO, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/archipelago-tiny.png "Type: ARCHIPELAGO, Size: TINY")

Type: CONTINENTS, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/continents-tiny.png "Type: CONTINENTS, Size: TINY")

Type: HIGHLAND, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/highland-tiny.png "Type: HIGHLAND, Size: TINY")

Type: INLAND_SEA, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/inland_sea-tiny.png "Type: INLAND_SEA, Size: TINY")

Type: ISLANDS, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/islands-tiny.png "Type: ISLANDS, Size: TINY")

Type: LAKES, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/lakes-tiny.png "Type: LAKES, Size: TINY")

Type: SMALL_CONTINENTS, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/small_continents-tiny.png "Type: SMALL_CONTINENTS, Size: TINY")

Type: SUPER_CONTINENT, Size: TINY
![Alt text](https://github.com/Ziagl/hex-map-generator/blob/main/example_images/super_continent-tiny.png "Type: SUPER_CONTINENT, Size: TINY")