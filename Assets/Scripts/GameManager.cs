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


    [Header("Prefabs")]
    public GameObject pulsePrefab;



    private int currentLevel = 1;
    private int score = 0;
    private GameState currentState = GameState.Playing;
    private PlayerController cachedPlayer;

    // ✅ Public properties
    public GameState CurrentState => currentState;
    public bool HasReachedGoal => currentState == GameState.Won;

    // 🔔 Optional: event for other systems (TutorialManager, Audio, etc.)
    public event System.Action OnGoalReachedEvent;
    public event System.Action OnLevelRestartedEvent;

    [SerializeField] private GameUIManager gameUIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //AdManager.Instance.ShowBanner(); ;

        StartLevel();

    }

    private void OnDestroy()
    {
        // Clear singleton reference when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // 🔁 Start or regenerate a level
    public void StartLevel()
    {
        Debug.Log($"🧠 Generating Level {currentLevel}...");
        SetState(GameState.Playing);

        // Reset gameplay visuals and map
        mapManager.GenerateMapAndSpawn();

        // Cache player reference after spawn
        cachedPlayer = FindObjectOfType<PlayerController>();

        // Reset UI
        gameUIManager.StartLevel();

        // Resume time
        Time.timeScale = 1f;
    }

    public void Transition()
    {
        gameUIManager.TransitionPanel();
    }
    // 🎯 When player reaches goal
    public void OnGoalReached()
    {
        if (currentState != GameState.Playing) return; // ✅ Prevent overlap
        SetState(GameState.Won);
        score += 100;

        Debug.Log($"🎉 Level {currentLevel} complete!");
        Time.timeScale = 0f;

        gameUIManager.HUD(currentLevel);
        gameUIManager.HudScore(score);
        gameUIManager.ShowWinPanel(score);

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
        gameUIManager.HUD(currentLevel);
        StartLevel();

    }

    // 💀 When energy depletes
    public void OnPlayerEnergyDepleted()
    {
        if (currentState != GameState.Playing) return; // ✅ Prevent overlap
        SetState(GameState.Lost);

        Debug.Log("💀 Player ran out of energy!");
        Time.timeScale = 0f;

        gameUIManager.ShowLossPanel("Player ran out of energy!");
    }

    // 🔄 Restart current level
    public void RestartLevel()
    {
        if (currentState == GameState.Transitioning) return;
        SetState(GameState.Transitioning);
        Time.timeScale = 1f;

        Debug.Log("🔄 Restarting current level...");
        AdManager.Instance.ShowInterstitial();
        StartLevel();



    }

    // 🧱 Restart entire game
    public void RestartGame()
    {
        Debug.Log("🔁 Restarting Game from Level 1...");
        AdManager.Instance.ShowInterstitial();
        gameUIManager.TransitionPanel();

    }

    public void Paused()
    {
        gameUIManager.PauseGame();
    }
    // 🎁 Continue after rewarded ad
    public void ContinueAfterAd()
    {
        AdManager.Instance.ShowRewarded();


    }

    public void GrantReward()
    {
        // Refill energy + hide panels
        if (cachedPlayer == null)
            cachedPlayer = FindObjectOfType<PlayerController>();

        cachedPlayer?.RestoreFullEnergy();

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
