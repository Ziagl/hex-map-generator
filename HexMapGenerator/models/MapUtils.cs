using com.hexagonsimulations.HexMapBase.Models;

namespace com.hexagonsimulations.HexMapGenerator.Models;

public class MapUtils
{
    /// <summary>
    /// Finds starting points for players on a given map. The starting points are
    /// equally distributed across the map.
    /// </summary>
    /// <param name="numPositions">Number of player starting positions needed.</param>
    /// <param name="map">A map of binary values meaning 0 unpassable and >0 passable.</param>
    /// <returns>Returns a list of computed starting positions.</returns>
    public static List<OffsetCoordinates> FindPlayerStartingPositions(int numPositions, List<int> map, int rows, int columns)
    {
        List<int[]> passablePositions = new List<int[]>();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (map[y * columns + x] != 0)
                {
                    passablePositions.Add(new int[] { x, y });
                }
            }
        }

        // K-Means algorithm
        double[][] centroids = Utils.InitializeCentroids(passablePositions.ToArray(), numPositions);
        double[][] oldCentroids;
        int[] labels = new int[passablePositions.Count];

        do
        {
            oldCentroids = (double[][])centroids.Clone();

            for (int i = 0; i < passablePositions.Count; i++)
            {
                labels[i] = Utils.FindClosestCentroid(passablePositions[i], centroids);
            }

            centroids = Utils.UpdateCentroids(passablePositions.ToArray(), labels, numPositions);

        } while (!Utils.AreCentroidsEqual(oldCentroids, centroids));

        List<OffsetCoordinates> startPositions = new List<OffsetCoordinates>();
        for (int i = 0; i < numPositions; i++)
        {
            startPositions.Add(
                Utils.FindNearestPosition(
                    centroids[i][0],
                    centroids[i][1],
                    passablePositions
                )
            );
        }

        return startPositions;
    }
}
