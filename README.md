# hex-map-generator
A hexagon map generator for Tiled Map Editor (https://www.mapeditor.org/) files.

## Sample of generated maps inside Tiled Map Editor:

Type: CONTINENTS_ISLANDS, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/continents_islands-tiny.png
![](example_images/continents_islands-tiny.png?raw=true "Type: CONTINENTS_ISLANDS, Size: TINY")

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
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/archipelago-tiny.png
![](example_images/archipelago-tiny.png?raw=true "Type: ARCHIPELAGO, Size: TINY")

Type: CONTINENTS, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/continents-tiny.png
![](example_images/continents-tiny.png?raw=true "Type: CONTINENTS, Size: TINY")

Type: HIGHLAND, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/highland-tiny.png
![](example_images/highland-tiny.png?raw=true "Type: HIGHLAND, Size: TINY")

Type: INLAND_SEA, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/inland_sea-tiny.png
![](example_images/inland_sea-tiny.png?raw=true "Type: INLAND_SEA, Size: TINY")

Type: ISLANDS, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/islands-tiny.png
![](example_images/islands-tiny.png?raw=true "Type: ISLANDS, Size: TINY")

Type: LAKES, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/lakes-tiny.png
![](example_images/lakes-tiny.png?raw=true "Type: LAKES, Size: TINY")

Type: SMALL_CONTINENTS, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/small_continents-tiny.png
![](example_images/small_continents-tiny.png?raw=true "Type: SMALL_CONTINENTS, Size: TINY")

Type: SUPER_CONTINENT, Size: TINY
https://github.com/Ziagl/hex-map-generator/blob/main/example_images/super_continent-tiny.png
![](example_images/super_continent-tiny.png?raw=true "Type: SUPER_CONTINENT, Size: TINY")