using UnityEngine;
using System.Collections.Generic;

public class EchoMapManager : MonoBehaviour
{
    [Header("References")]
    public EchoMapGenerator generator;
    public EchoMapRenderer rend;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject goalPrefab;

    [Header("Parent Containers")]
    public Transform mapParent;
    public Transform playerParent;

    [Header("Tile Settings")]
    public Vector2 tileSize = Vector2.one;

    [Header("UI References")]
    public Joystick joystick;

    private EchoMap currentMap;
    private GameObject currentPlayer;
    private GameObject currentGoal;

    private const int MAX_RETRY_ATTEMPTS = 5;

    // 💡 Main entry point — can be reused for next levels
    public void GenerateMapAndSpawn()
    {
        StartCoroutine(GenerateMapRoutine());
    }

    private System.Collections.IEnumerator GenerateMapRoutine()
    {
        int attempt = 0;
        bool validMap = false;

        while (attempt < MAX_RETRY_ATTEMPTS && !validMap)
        {
            attempt++;
            Debug.Log($"🧩 Generating map attempt {attempt}...");

            ClearPreviousMap();

            generator.seed = Random.Range(0, 999999);
            currentMap = generator.Generate();
            rend.Render(currentMap);

            // Try spawning player + goal
            validMap = TrySpawnPlayerAndGoal();

            if (!validMap)
            {
                Debug.LogWarning($"⚠️ No valid path found on attempt {attempt}. Retrying...");
                yield return null;
            }
        }

        if (!validMap)
        {
            Debug.LogError("❌ Failed to generate a valid map after multiple attempts!");
            // Fallback to a minimal map or safe center spawn
            SpawnFallbackScenario();
        }
        else
        {
            Debug.Log("✅ Valid playable map generated successfully!");
        }
    }

    // 🧹 Clears previous map, player, and goal
    private void ClearPreviousMap()
    {
        if (mapParent != null)
        {
            foreach (Transform child in mapParent)
                Destroy(child.gameObject);
        }

        if (playerParent != null)
        {
            foreach (Transform child in playerParent)
                Destroy(child.gameObject);
        }

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        if (currentGoal != null)
        {
            Destroy(currentGoal);
            currentGoal = null;
        }
    }

    // 🎯 Attempts to spawn player and goal ensuring path connectivity
    private bool TrySpawnPlayerAndGoal()
    {
        // Collect all empty tiles
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int x = 1; x < currentMap.width - 1; x++)
        {
            for (int y = 1; y < currentMap.height - 1; y++)
            {
                if (currentMap.tiles[x, y] == TileType.Empty)
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        if (emptyTiles.Count < 2)
        {
            Debug.LogWarning("⚠️ Not enough empty tiles to place player and goal!");
            return false;
        }

        emptyTiles.Shuffle();

        // Randomly choose player and goal, ensure reachable
        foreach (Vector2Int playerTile in emptyTiles)
        {
            foreach (Vector2Int goalTile in emptyTiles)
            {
                if (playerTile == goalTile) continue;

                if (MapUtils.IsReachable(currentMap, playerTile, goalTile))
                {
                    SpawnPlayerAtTile(playerTile);
                    SpawnGoalAtTile(goalTile);
                    return true;
                }
            }
        }

        return false;
    }

    private void SpawnPlayerAtTile(Vector2Int tile)
    {
        Vector3 spawnWorld = MapUtils.TileToWorld(tile, tileSize);
        currentPlayer = Instantiate(playerPrefab, spawnWorld, Quaternion.identity, playerParent);

        // ✅ Assign camera target
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
            cameraFollow.SetTarget(currentPlayer.transform);

        // ✅ Assign joystick
        var playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.AssignJoystick(joystick);

        Debug.Log($"🧍 Player spawned at {tile}");
    }

    private void SpawnGoalAtTile(Vector2Int tile)
    {
        currentMap.tiles[tile.x, tile.y] = TileType.Goal;
        Vector3 goalWorld = MapUtils.TileToWorld(tile, tileSize);
        currentGoal = Instantiate(goalPrefab, goalWorld, Quaternion.identity, mapParent);

        if (currentPlayer != null)
        {
            GoalController goalCtrl = currentGoal.GetComponent<GoalController>();
            if (goalCtrl != null)
                goalCtrl.AssignPlayer(currentPlayer.transform);
                ThemeSongGenerator.Instance?.SetGoal(goalCtrl.transform);

            }
        }

        Debug.Log($"🎯 Goal placed at {tile}");
    }

    // 🆘 Fallback if generation fails completely
    private void SpawnFallbackScenario()
    {
        Debug.LogWarning("⚠️ Spawning fallback scenario...");

        ClearPreviousMap();

        Vector3 fallbackPos = new Vector3(0, 0, 0);
        currentPlayer = Instantiate(playerPrefab, fallbackPos, Quaternion.identity, playerParent);

        Vector3 goalPos = fallbackPos + new Vector3(3f, 0f, 0f);
        currentGoal = Instantiate(goalPrefab, goalPos, Quaternion.identity, mapParent);

        var cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.SetTarget(currentPlayer.transform);

        var playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.AssignJoystick(joystick);
    }
}
