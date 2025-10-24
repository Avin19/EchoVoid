using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private VisualElement root;
    private Button pauseButton;
    private VisualElement pausePanel;
    private Button resumeButton;
    private Button mainMenuButton;
    private Button exitButton;

    private bool isPaused = false;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        pauseButton = root.Q<Button>("pause-button");
        pausePanel = root.Q<VisualElement>("pause-panel");
        resumeButton = root.Q<Button>("resume-button");
        mainMenuButton = root.Q<Button>("mainmenu-button");
        exitButton = root.Q<Button>("exit-button");

        pauseButton.clicked += OnPauseClicked;
        resumeButton.clicked += OnResumeClicked;
        mainMenuButton.clicked += OnMainMenuClicked;
        exitButton.clicked += OnExitClicked;
    }

    private void OnPauseClicked()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.style.display = DisplayStyle.Flex;
        pauseButton.style.display = DisplayStyle.None;
        ThemeSongGenerator.Instance?.GetComponent<AudioSource>().Pause();
    }

    private void OnResumeClicked()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.style.display = DisplayStyle.None;
        pauseButton.style.display = DisplayStyle.Flex;
        ThemeSongGenerator.Instance?.GetComponent<AudioSource>().Play();
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnExitClicked()
    {
        Debug.Log("🚪 Exiting game...");
        Application.Quit();
    }
}
