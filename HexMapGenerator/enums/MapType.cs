﻿namespace com.hexagonsimulations.HexMapGenerator.Enums;

public enum MapType
{
    RANDOM = 0, // only to test
    ARCHIPELAGO = 1, // lots of small islands
    INLAND_SEA = 2, // basic plain with giant sea in the middle
    HIGHLAND = 3, // basic plain with lots of hill / mountain structures and lakes
    ISLANDS = 4, // islands of different size
    SMALL_CONTINENTS = 5, // like islands, but equal big landmasses
    CONTINENTS = 6, // few but big continents
    CONTINENTS_ISLANDS = 7, // few big continents and some islands
    SUPER_CONTINENT = 8, // one giant continent surrounded by water
    LAKES = 9, // basic plain with lots of small and big lakes
}
