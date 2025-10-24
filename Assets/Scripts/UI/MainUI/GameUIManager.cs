using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    private VisualElement root;

    private VisualElement hudRoot, pausePanel, winRoot, tutorialRoot, energyFill;
    private Button pauseButton, resumeButton, mainMenuButton, exitButton, nextLevelButton, rewardButton;
    private Label levelLabel, scoreLabel, winScoreLabel, tutorialText, hintLabel;

    private bool isPaused = false;

    void Awake() => Instance = this;

    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Query all panels
        hudRoot = root.Q<VisualElement>("hud-root");
        pausePanel = root.Q<VisualElement>("pause-panel");
        winRoot = root.Q<VisualElement>("win-root");
        tutorialRoot = root.Q<VisualElement>("tutorial-root");

        // HUD
        levelLabel = root.Q<Label>("level-label");
        scoreLabel = root.Q<Label>("score-label");
        energyFill = root.Q<VisualElement>("energy-bar-fill");

        // Pause
        pauseButton = root.Q<Button>("pause-button");
        resumeButton = root.Q<Button>("resume-button");
        mainMenuButton = root.Q<Button>("mainmenu-button");
        exitButton = root.Q<Button>("exit-button");

        // Win
        winScoreLabel = root.Q<Label>("win-score");
        nextLevelButton = root.Q<Button>("next-level-button");
        rewardButton = root.Q<Button>("reward-button");

        // Tutorial
        tutorialText = root.Q<Label>("tutorial-text");
        hintLabel = root.Q<Label>("hint-label");

        // Setup events
        pauseButton.clicked += PauseGame;
        resumeButton.clicked += ResumeGame;
        mainMenuButton.clicked += () => SceneManager.LoadScene(0);
        exitButton.clicked += Application.Quit;
        nextLevelButton.clicked += () => GameManager.Instance?.NextLevel();

    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.style.display = DisplayStyle.Flex;
        pauseButton.style.display = DisplayStyle.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.style.display = DisplayStyle.None;
        pauseButton.style.display = DisplayStyle.Flex;
    }

    public void ShowWinPanel(int score)
    {
        winScoreLabel.text = $"SCORE: {score:0000}";
        winRoot.style.display = DisplayStyle.Flex;
    }

    public void ShowTutorial(string text)
    {
        tutorialText.text = text;
        tutorialRoot.style.display = DisplayStyle.Flex;
    }

    public void HideTutorial()
    {
        tutorialRoot.style.display = DisplayStyle.None;
    }
}
