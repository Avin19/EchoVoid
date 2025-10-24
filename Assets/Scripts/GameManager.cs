using UnityEngine;
using System.Collections;

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
        if (currentState != GameState.Playing) return;
        SetState(GameState.Won);

        Debug.Log($"🎉 Level {currentLevel} complete!");
        StartCoroutine(ShowWinPanelDelayed());
    }

    private IEnumerator ShowWinPanelDelayed()
    {
        yield return new WaitForSeconds(0.5f); // small delay for effect

        int score = UIManager.Instance != null ? UIManager.Instance.CurrentScore : 0;
        LossPanelManager.Instance?.HideLossPanel();
        WinPanelManager.Instance?.ShowWinPanel(score);
        OnGoalReachedEvent?.Invoke();
        Time.timeScale = 0f;
    }


    // ⚡ Move to the next level
    public void NextLevel()
    {
        if (currentState == GameState.Transitioning) return;
        SetState(GameState.Transitioning);

        // ✅ Make sure the game isn’t paused before transition
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

        SetState(GameState.Playing);
        Time.timeScale = 1f;

        var player = FindObjectOfType<PlayerController>();
        player?.RestoreFullEnergy();

        WinPanelManager.Instance?.HideWinPanel();
        LossPanelManager.Instance?.HideLossPanel();

        // ✅ Ensure level isn’t frozen in post-win mode
        if (HasReachedGoal)
            StartLevel();
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
    [ContextMenu("Force Replay Level")]
    public void DebugReplayLevel()
    {
        Debug.Log("🧪 Replaying current level manually...");
        SetState(GameState.Playing);
        StartLevel();
    }
}
