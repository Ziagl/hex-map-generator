using com.hexagonsimulations.HexMapBase.Models;
using com.hexagonsimulations.HexMapGenerator.Models;

namespace com.hexagonsimulations.HexMapGenerator.Tests;

[TestClass]
public sealed class MapUtilsTests
{
    [TestMethod]
    public void TestFindPlayerStartingPositions()
    {
        List<int> map = new()
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };
        var startPositions = MapUtils.FindPlayerStartingPositions(2, map, 10, 10);
        Assert.HasCount(2, startPositions);
        Assert.IsGreaterThan(4.0, Distance(startPositions[0], startPositions[1]));

        map = new()
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };
        startPositions = MapUtils.FindPlayerStartingPositions(2, map, 5, 10);
        Assert.HasCount(2, startPositions);

        // because of randomness try this complex example 10 times
        for (int x = 0; x < 10; ++x)
        {
            map = new()
            {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                0, 0, 1, 1, 1, 1, 1, 0, 0, 1,
                1, 0, 0, 1, 1, 1, 0, 0, 0, 1,
                1, 1, 1, 1, 1, 1, 0, 0, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 0, 0, 1, 1, 1, 1,
                1, 1, 1, 1, 0, 0, 0, 1, 1, 1,
                1, 1, 1, 1, 1, 0, 0, 1, 1, 1
            };
            startPositions = MapUtils.FindPlayerStartingPositions(5, map, 10, 10);
            Assert.HasCount(5, startPositions);
            List<double> distances = new();
            double minDistance = double.MaxValue;
            double maxDistance = double.MinValue;
            for (int i = 0; i < startPositions.Count(); ++i)
            {
                for (int j = 0; j < startPositions.Count(); ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    double distance = Distance(startPositions[i], startPositions[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                    distances.Add(distance);
                }
                Assert.IsGreaterThan(0, map[startPositions[i].y * 10 + startPositions[i].x], "Map position is 0");
            }
            Assert.IsLessThan(7.0, maxDistance - minDistance, $"Difference is {maxDistance - minDistance}");
            Assert.IsGreaterThanOrEqualTo(2.0, minDistance, $"Distance is {minDistance}");
        }
    }

    private double Distance(OffsetCoordinates a, OffsetCoordinates b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }
}
