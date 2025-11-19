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

    private EchoMap currentMap;
    private GameObject currentPlayer;
    private GameObject currentGoal;
    [Header("UI References")]
    public Joystick joystick;
    void Start()
    {
        // GenerateMapAndSpawn();
    }

    // 💡 Main entry point — can be reused for next levels
    public void GenerateMapAndSpawn()
    {
        if (generator == null)
        {
            Debug.LogError("EchoMapManager: Generator is null! Cannot generate map.");
            return;
        }

        if (rend == null)
        {
            Debug.LogError("EchoMapManager: Renderer is null! Cannot render map.");
            return;
        }

        // Clear previous map objects
        if (mapParent != null)
        {
            foreach (Transform child in mapParent)
                Destroy(child.gameObject);
        }

        // 🔥 Ensure old player is removed
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        if (playerParent != null)
        {
            foreach (Transform child in playerParent)
                Destroy(child.gameObject);
        }

        if (currentGoal != null)
        {
            Destroy(currentGoal);
            currentGoal = null;
        }

        // Generate and render new map
        generator.seed = Random.Range(0, 999999);
        currentMap = generator.Generate();
        rend.Render(currentMap);

        // Place player first (before goal)
        SpawnPlayerAtValidTile();

        // Then place goal (so it gets player reference)
        SpawnGoalAtValidTile();
    }


    // 🎯 Spawns the player in a reachable, empty tile
    void SpawnPlayerAtValidTile()
    {
        Vector2Int goal = MapUtils.FindTile(currentMap, TileType.Goal);
        if (goal.x == -1) goal = new Vector2Int(currentMap.width - 3, currentMap.height - 3);

        Vector3 spawnPos = Vector3.zero;
        bool foundValidTile = false;

        for (int x = 1; x < currentMap.width - 1; x++)
        {
            for (int y = 1; y < currentMap.height - 1; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                if (currentMap.tiles[x, y] == TileType.Empty)
                {
                    if (MapUtils.IsReachable(currentMap, tile, goal))
                    {
                        spawnPos = MapUtils.TileToWorld(tile, tileSize);
                        foundValidTile = true;
                        Debug.Log($"✅ Player spawned at tile {tile}");
                        break;
                    }
                }
            }
            if (foundValidTile) break;
        }

        // Fallback center spawn if no valid tile found
        if (!foundValidTile)
        {
            spawnPos = new Vector3(currentMap.width / 2, currentMap.height / 2, 0);
            Debug.Log("⚠️ Player spawned at fallback center");
        }

        // Instantiate player
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity, playerParent);
        
        // Setup player references (consolidated)
        SetupPlayerReferences(currentPlayer);
    }

    // 🔧 Helper method to setup player references (eliminates duplication)
    void SetupPlayerReferences(GameObject player)
    {
        if (player == null) return;

        // Assign camera target
        var cameraFollow = Camera.main?.GetComponent<CameraFollow>();
        if (cameraFollow != null)
            cameraFollow.SetTarget(player.transform);

        // Assign joystick reference
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null && joystick != null)
        {
            playerController.AssignJoystick(joystick);
        }

        // Add goal direction arrow
        var directionArrow = player.GetComponent<GoalDirectionArrow>();
        if (directionArrow == null)
        {
            directionArrow = player.AddComponent<GoalDirectionArrow>();
        }
        directionArrow.SetPlayer(player.transform);
        
        // Goal will be set after it's spawned
    }

    // 🌟 Spawns goal on an empty tile
    void SpawnGoalAtValidTile()
    {
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int x = 2; x < currentMap.width - 2; x++)
        {
            for (int y = 2; y < currentMap.height - 2; y++)
            {
                if (currentMap.tiles[x, y] == TileType.Empty)
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        if (emptyTiles.Count == 0)
        {
            Debug.LogWarning("⚠️ No empty tiles available for goal placement!");
            return;
        }

        emptyTiles.Shuffle();

        Vector2Int goalTile = emptyTiles[0];
        currentMap.tiles[goalTile.x, goalTile.y] = TileType.Goal;

        Vector3 goalWorld = MapUtils.TileToWorld(goalTile, tileSize);
        GameObject goal = Instantiate(goalPrefab, goalWorld, Quaternion.identity, mapParent);

        // ✅ Pass player reference if already spawned
        if (currentPlayer != null)
        {
            GoalController goalCtrl = goal.GetComponent<GoalController>();
            if (goalCtrl != null)
            {
                goalCtrl.AssignPlayer(currentPlayer.transform);
                ThemeSongGenerator.Instance?.SetGoal(goalCtrl.transform);
            }

            // Set goal reference for direction arrow
            var directionArrow = currentPlayer.GetComponent<GoalDirectionArrow>();
            if (directionArrow != null)
            {
                directionArrow.SetGoal(goal.transform);
            }
        }

        currentGoal = goal;
        Debug.Log($"🎯 Goal placed at tile {goalTile}");
    }

}
