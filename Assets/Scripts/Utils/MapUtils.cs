using UnityEngine;
using System.Collections.Generic;

public static class MapUtils
{
    public static bool IsReachable(EchoMap map, Vector2Int start, Vector2Int target)
    {
        if (!InBounds(map, start) || !InBounds(map, target)) return false;
        if (map.tiles[start.x, start.y] != TileType.Empty) return false;
        if (map.tiles[target.x, target.y] == TileType.Wall) return false;

        bool[,] visited = new bool[map.width, map.height];
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(start);
        visited[start.x, start.y] = true;

        int[] dx = new int[] { 1, -1, 0, 0 };
        int[] dy = new int[] { 0, 0, 1, -1 };

        while (q.Count > 0)
        {
            var p = q.Dequeue();
            if (p == target) return true;

            for (int i = 0; i < 4; i++)
            {
                int nx = p.x + dx[i];
                int ny = p.y + dy[i];

                if (nx >= 0 && ny >= 0 && nx < map.width && ny < map.height && !visited[nx, ny])
                {
                    if (map.tiles[nx, ny] == TileType.Empty || map.tiles[nx, ny] == TileType.Goal)
                    {
                        visited[nx, ny] = true;
                        q.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
        return false;
    }

    public static bool InBounds(EchoMap map, Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < map.width && p.y < map.height;
    }
    // 🔍 Finds the first tile of a given type in the map
    public static Vector2Int FindTile(EchoMap map, TileType type)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                if (map.tiles[x, y] == type)
                    return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }

    // 🎲 Picks a random empty tile (safe spawn or placement)
    public static Vector2Int GetRandomEmptyTile(EchoMap map)
    {
        List<Vector2Int> emptyTiles = new List<Vector2Int>();

        for (int x = 1; x < map.width - 1; x++)
        {
            for (int y = 1; y < map.height - 1; y++)
            {
                if (map.tiles[x, y] == TileType.Empty)
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        if (emptyTiles.Count == 0)
            return new Vector2Int(-1, -1);

        return emptyTiles[Random.Range(0, emptyTiles.Count)];
    }

    // 🧭 Converts grid tile coordinates → Unity world position
    public static Vector3 TileToWorld(Vector2Int tile, Vector2 tileSize)
    {
        float worldX = tile.x * tileSize.x;
        float worldY = tile.y * tileSize.y;

        // Optionally center objects on their tile
        // worldX += tileSize.x * 0.5f;
        // worldY += tileSize.y * 0.5f;

        return new Vector3(worldX, worldY, 0f);
    }

    // 🔄 Converts world position → nearest tile grid coordinates (optional helper)
    public static Vector2Int WorldToTile(Vector3 worldPos, Vector2 tileSize)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize.x);
        int y = Mathf.RoundToInt(worldPos.y / tileSize.y);
        return new Vector2Int(x, y);
    }
}
