using UnityEngine;
using System.Collections.Generic;

public class EchoMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 64;
    public int height = 64;
    [Range(0, 100)] public int fillPercent = 45;
    public int smoothSteps = 5;
    public int seed;

    public EchoMap Generate()
    {
        EchoMap map = new EchoMap(width, height);
        Random.InitState(seed);

        // Step 1. Random Fill
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    map.tiles[x, y] = TileType.Wall;
                else
                    map.tiles[x, y] = (Random.Range(0, 100) < fillPercent) ? TileType.Wall : TileType.Empty;
            }
        }

        // Step 2. Smooth Iterations
        for (int i = 0; i < smoothSteps; i++)
            SmoothMap(map);

        // Step 3. Place Goal

        return map;
    }

    void SmoothMap(EchoMap map)
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int wallCount = GetNeighborWallCount(map, x, y);

                if (wallCount > 4)
                    map.tiles[x, y] = TileType.Wall;
                else if (wallCount < 4)
                    map.tiles[x, y] = TileType.Empty;
            }
        }
    }

    int GetNeighborWallCount(EchoMap map, int x, int y)
    {
        int count = 0;
        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    if (nx != x || ny != y)
                        if (map.tiles[nx, ny] == TileType.Wall)
                            count++;
                }
            }
        }
        return count;
    }

    void PlaceGoal(EchoMap map)
    {
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int x = 2; x < width - 2; x++)
        {
            for (int y = 2; y < height - 2; y++)
            {
                if (map.tiles[x, y] == TileType.Empty)
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        if (emptyTiles.Count == 0) return;

        // find player tile (approx center)
        Vector2Int playerStart = new Vector2Int(width / 2, height / 2);

        // shuffle list to randomize selection
        emptyTiles.Shuffle(); // create an extension for this if needed

        foreach (var tile in emptyTiles)
        {
            if (MapUtils.IsReachable(map, playerStart, tile))
            {
                map.tiles[tile.x, tile.y] = TileType.Goal;
                Debug.Log($"Goal placed at {tile}");
                return;
            }
        }

        // fallback (should be rare)
        Vector2Int fallback = emptyTiles[Random.Range(0, emptyTiles.Count)];
        map.tiles[fallback.x, fallback.y] = TileType.Goal;
    }

}
