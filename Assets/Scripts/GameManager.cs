using UnityEngine;

public enum GameState
{
    Playing,
    Won,
    Lost,
    Transitioning
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public EchoMapManager mapManager;

    [Header("Settings")]
    public float levelTransitionDelay = 1.5f;

    [Header("Prefabs")]
    public GameObject pulsePrefab;

    private int currentLevel = 1;
    private GameState currentState = GameState.Playing;

    // ✅ Public properties
    public GameState CurrentState => currentState;
    public bool HasReachedGoal => currentState == GameState.Won;

    // 🔔 Optional: event for other systems (TutorialManager, Audio, etc.)
    public event System.Action OnGoalReachedEvent;
    public event System.Action OnLevelRestartedEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartLevel();
    }

    // 🔁 Start or regenerate a level
    public void StartLevel()
    {
        Debug.Log($"🧠 Generating Level {currentLevel}...");
        SetState(GameState.Playing);

        // Reset gameplay visuals and map
        mapManager.GenerateMapAndSpawn();

        // Reset UI
        UIManager.Instance?.ResetUIForNewLevel();

        // Resume time
        Time.timeScale = 1f;
    }

    // 🎯 When player reaches goal
    public void OnGoalReached()
    {
        if (currentState != GameState.Playing) return; // ✅ Prevent overlap
        SetState(GameState.Won);

        Debug.Log($"🎉 Level {currentLevel} complete!");
        Time.timeScale = 0f;

        int score = UIManager.Instance != null ? UIManager.Instance.CurrentScore : 0;

        // Hide any active loss UI (safety)
        LossPanelManager.Instance?.HideLossPanel();

        // Show win panel
        WinPanelManager.Instance?.ShowWinPanel(score);

        // 🔔 Notify any listeners
        OnGoalReachedEvent?.Invoke();
    }

    // ⚡ Move to the next level
    public void NextLevel()
    {
        if (currentState == GameState.Transitioning) return;
        SetState(GameState.Transitioning);
        Time.timeScale = 1f;

        currentLevel++;
        Debug.Log($"🚀 Loading Level {currentLevel}...");

        LevelTransitionManager.Instance?.FadeToNextLevel(() =>
        {
            UIManager.Instance?.UpdateLevel(currentLevel);
            StartLevel();
        });
    }

    // 💀 When energy depletes
    public void OnPlayerEnergyDepleted()
    {
        if (currentState != GameState.Playing) return; // ✅ Prevent overlap
        SetState(GameState.Lost);

        Debug.Log("💀 Player ran out of energy!");
        Time.timeScale = 0f;

        // Hide WinPanel if somehow active
        WinPanelManager.Instance?.HideWinPanel();

        // Show loss panel
        LossPanelManager.Instance?.ShowLossPanel();
    }

    // 🔄 Restart current level
    public void RestartLevel()
    {
        if (currentState == GameState.Transitioning) return;
        SetState(GameState.Transitioning);
        Time.timeScale = 1f;

        Debug.Log("🔄 Restarting current level...");

        LossPanelManager.Instance?.HideLossPanel();

        LevelTransitionManager.Instance?.FadeToNextLevel(() =>
        {
            StartLevel();
            OnLevelRestartedEvent?.Invoke(); // Notify listeners (e.g., tutorial reset)
        });
    }

    // 🧱 Restart entire game
    public void RestartGame()
    {
        Debug.Log("🔁 Restarting Game from Level 1...");
        currentLevel = 1;
        StartLevel();
    }

    // 🎁 Continue after rewarded ad
    public void ContinueAfterAd()
    {
        Debug.Log("🎁 Continue after ad watched!");

        // Reset state
        SetState(GameState.Playing);
        Time.timeScale = 1f;

        // Refill energy + hide panels
        var player = FindObjectOfType<PlayerController>();
        player?.RestoreFullEnergy();

        WinPanelManager.Instance?.HideWinPanel();
        LossPanelManager.Instance?.HideLossPanel();
    }

    // 👀 Helper for player input
    public bool IsPlaying() => currentState == GameState.Playing;

    // ⚙️ State setter (ensures consistency + logs changes)
    private void SetState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log($"📜 GameState changed to: {currentState}");
    }
}
