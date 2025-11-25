using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    private VisualElement root;
    private Button pauseButton;
    private VisualElement pausePanel;
    private Button resumeButton;
    private Button restartButton;
    private Button mainMenuButton;
    private Button exitButton;

    private bool isPaused = false;
    
    [Header("Settings")]
    public bool pauseOnEscape = true;
    public bool allowPauseDuringTransition = false;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set proper sorting order for pause panel
        var uiDoc = GetComponent<UIDocument>();
        // if (uiDoc != null && UIPanelSortingManager.Instance != null)
        // {
        //     UIPanelSortingManager.Instance.SetPanelSortOrder(uiDoc, PanelType.Pause);
        // }
    }

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        pauseButton = root.Q<Button>("pause-button");
        pausePanel = root.Q<VisualElement>("pause-panel");
        resumeButton = root.Q<Button>("resume-button");
        restartButton = root.Q<Button>("restart-button");
        mainMenuButton = root.Q<Button>("mainmenu-button");
        exitButton = root.Q<Button>("exit-button");

        // Hook up button events
        if (pauseButton != null)
            pauseButton.clicked += OnPauseClicked;
        if (resumeButton != null)
            resumeButton.clicked += OnResumeClicked;
        if (restartButton != null)
            restartButton.clicked += OnRestartClicked;
        if (mainMenuButton != null)
            mainMenuButton.clicked += OnMainMenuClicked;
        if (exitButton != null)
            exitButton.clicked += OnExitClicked;

        // Ensure panel is hidden at start
        if (pausePanel != null)
            pausePanel.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        // Handle ESC key for pause
        if (pauseOnEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                OnResumeClicked();
            else
                OnPauseClicked();
        }
    }

    private void OnDestroy()
    {
        // Clear singleton reference when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
    }

    private void OnPauseClicked()
    {
        // Check if pausing is allowed
        if (!allowPauseDuringTransition && GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentState == GameState.Transitioning)
            {
                Debug.Log("⚠️ Cannot pause during level transition");
                return;
            }
        }

        PauseGame();
    }

    private void OnResumeClicked()
    {
        ResumeGame();
    }

    private void OnRestartClicked()
    {
        Debug.Log("🔄 Restarting level from pause menu...");
        ResumeGame();
        GameManager.Instance?.RestartLevel();
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("🏠 Returning to main menu...");
        
        // Clean up before scene change
        CleanupBeforeSceneChange();
        
        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }

    private void CleanupBeforeSceneChange()
    {
        // Reset time scale
        Time.timeScale = 1f;
        
        // Resume audio if paused
        if (ThemeSongGenerator.Instance != null)
        {
            var audioSource = ThemeSongGenerator.Instance.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.Stop();
        }
        
        // Clear pause state
        isPaused = false;
        
        // Clear singleton reference
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnExitClicked()
    {
        Debug.Log("🚪 Exiting game...");
        Time.timeScale = 1f;
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Public methods for external control
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
            pausePanel.style.display = DisplayStyle.Flex;
        if (pauseButton != null)
            pauseButton.style.display = DisplayStyle.None;

        // Pause audio
        if (ThemeSongGenerator.Instance != null)
        {
            var audioSource = ThemeSongGenerator.Instance.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.Pause();
        }

        Debug.Log("⏸️ Game paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
            pausePanel.style.display = DisplayStyle.None;
        if (pauseButton != null)
            pauseButton.style.display = DisplayStyle.Flex;

        // Resume audio
        if (ThemeSongGenerator.Instance != null)
        {
            var audioSource = ThemeSongGenerator.Instance.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.UnPause();
        }

        Debug.Log("▶️ Game resumed");
    }

    public bool IsPaused => isPaused;
}
