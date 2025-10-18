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
        Assert.AreEqual(2, startPositions.Count);
        Assert.IsTrue(Distance(startPositions[0], startPositions[1]) > 4.0);

        map = new()
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };
        startPositions = MapUtils.FindPlayerStartingPositions(2, map, 5, 10);
        Assert.AreEqual(2, startPositions.Count);

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
            Assert.AreEqual(5, startPositions.Count);
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
                Assert.IsTrue(map[startPositions[i].y * 10 + startPositions[i].x] > 0, "Map position is 0");
            }
            Assert.IsTrue(maxDistance - minDistance < 7.0, $"Difference is {maxDistance - minDistance}");
            Assert.IsTrue(minDistance >= 2.0, $"Distance is {minDistance}");
        }
    }

    private double Distance(OffsetCoordinates a, OffsetCoordinates b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }
}
